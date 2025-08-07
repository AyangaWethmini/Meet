using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SafeMeet.Api.Models;
using SafeMeet.Api.Services;
using System.Security.Claims;

namespace SafeMeet.Api.Controllers
{
    [ApiController]
    [Route("api/Availability")]
    [Authorize]
    public class AvailabilityController : ControllerBase
    {
        private readonly IMongoCollection<AvailabilitySlot> _availabilitySlots;
        private readonly UserService _userService;

        public AvailabilityController(MongoDbService mongoDbService, UserService userService)
        {
            _availabilitySlots = mongoDbService.GetCollection<AvailabilitySlot>("AvailabilitySlots");
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(AvailabilitySlot slot)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            slot.UserId = userId;
            await _availabilitySlots.InsertOneAsync(slot);
            return CreatedAtAction(nameof(GetById), new { id = slot.Id }, slot);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var slots = await _availabilitySlots.Find(s => s.UserId == userId).ToListAsync();
            return Ok(slots);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var slot = await _availabilitySlots.Find(s => s.Id == id && s.UserId == userId).FirstOrDefaultAsync();
            if (slot == null) return NotFound();
            return Ok(slot);
        }

        [HttpGet("user/{userid}")]
        public async Task<IActionResult> GetByUser(string userid)
        {
            var slots = await _availabilitySlots.Find(s => s.UserId == userid).ToListAsync();
            return Ok(slots);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, AvailabilitySlot updatedSlot)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var existing = await _availabilitySlots.Find(s => s.Id == id && s.UserId == userId).FirstOrDefaultAsync();
            if (existing == null) return NotFound();

            updatedSlot.Id = id;
            updatedSlot.UserId = userId;
            var result = await _availabilitySlots.ReplaceOneAsync(s => s.Id == id, updatedSlot);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _availabilitySlots.DeleteOneAsync(s => s.Id == id && s.UserId == userId);
            if (result.DeletedCount == 0) return NotFound();

            return NoContent();
        }
    }
}
