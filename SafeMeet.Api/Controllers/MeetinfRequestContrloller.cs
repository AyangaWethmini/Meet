using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SafeMeet.Api.Models;
using SafeMeet.Api.Services;
using System.Security.Claims;


namespace SafeMeet.Api.Controllers
{
    [ApiController]
    [Route("api/request")]
    [Authorize]
    public class MeetingRequestController : ControllerBase
    {
        private readonly IMongoCollection<MeetingRequest> _meetingRequestCollection;
        private readonly UserService _userService;

        public MeetingRequestController(MongoDbService mongoDbService, UserService userService)
        {
            _meetingRequestCollection = mongoDbService.GetCollection<MeetingRequest>("MeetingRequests");
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> createRequest(MeetingRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (request.Attendees == null || !request.Attendees.Any()) return BadRequest("Atleast on invitee is required");

            request.CreatedBy = userId;
            await _meetingRequestCollection.InsertOneAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
        }


        //get request by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var request = await _meetingRequestCollection
                .Find(r => r.Id == id && (r.CreatedBy == userId || r.Attendees.Contains(userEmail)))
                .FirstOrDefaultAsync();

            if (request == null) return NotFound();
            return Ok(request);
        }


        //get all the requests of an authenticated user
        // Get all meeting requests for the authenticated user (as creator or invitee)
             [HttpGet("my-requests")]
             public async Task<IActionResult> GetMyRequests()
             {
                 var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                 var email = User.FindFirst(ClaimTypes.Email)?.Value;
                 if (string.IsNullOrEmpty(userId)) return Unauthorized();

                 var requests = await _meetingRequestCollection
                     .Find(r => r.CreatedBy == userId || r.Attendees.Contains(email))
                     .ToListAsync();

                 return Ok(requests);
             }

             // Update a meeting request
             [HttpPut("{id}")]
             public async Task<IActionResult> Update(string id, MeetingRequest updatedRequest)
             {
                 var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                 if (string.IsNullOrEmpty(userId)) return Unauthorized();

                 var existing = await _meetingRequestCollection
                     .Find(r => r.Id == id && r.CreatedBy == userId)
                     .FirstOrDefaultAsync();

                 if (existing == null) return NotFound();

                 // Validate Attendees
                 if (updatedRequest.Attendees == null || !updatedRequest.Attendees.Any())
                     return BadRequest("At least one invitee email is required.");

                 updatedRequest.Id = id;
                 updatedRequest.CreatedBy = userId;
                 var result = await _meetingRequestCollection.ReplaceOneAsync(r => r.Id == id, updatedRequest);

                 if (result.MatchedCount == 0) return NotFound();
                 return NoContent();
             }

             // Delete a meeting request
             [HttpDelete("{id}")]
             public async Task<IActionResult> Delete(string id)
             {
                 var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                 if (string.IsNullOrEmpty(userId)) return Unauthorized();

                 var result = await _meetingRequestCollection
                     .DeleteOneAsync(r => r.Id == id && r.CreatedBy == userId);

                 if (result.DeletedCount == 0) return NotFound();
                 return NoContent();
             }
    }

}