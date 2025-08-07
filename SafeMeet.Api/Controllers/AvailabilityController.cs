using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly AvailabilityService _availabilityService;
        private readonly UserService _userService;

        public AvailabilityController(AvailabilityService availabilityService, UserService userService)
        {
            _availabilityService = availabilityService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(AvailabilitySlot slot)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            slot.UserId = userId;
            var createdSlot = await _availabilityService.CreateAvailabilitySlotAsync(slot);
            return CreatedAtAction(nameof(GetById), new { id = createdSlot.Id }, createdSlot);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var slots = await _availabilityService.GetAvailabilityByUserIdAsync(userId);
            return Ok(slots);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var slot = await _availabilityService.GetAvailabilitySlotByIdAsync(id, userId);
            if (slot == null) return NotFound();
            return Ok(slot);
        }

        [HttpGet("user/{userid}")]
        public async Task<IActionResult> GetByUser(string userid)
        {
            var slots = await _availabilityService.GetAvailabilityByUserIdAsync(userid);
            return Ok(slots);
        }

        [HttpGet("daterange")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var slots = await _availabilityService.GetAvailabilityByDateRangeAsync(userId, startDate, endDate);
            return Ok(slots);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, AvailabilitySlot updatedSlot)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var existing = await _availabilityService.GetAvailabilitySlotByIdAsync(id, userId);
            if (existing == null) return NotFound();

            var success = await _availabilityService.UpdateAvailabilitySlotAsync(id, userId, updatedSlot);
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _availabilityService.DeleteAvailabilitySlotAsync(id, userId);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
