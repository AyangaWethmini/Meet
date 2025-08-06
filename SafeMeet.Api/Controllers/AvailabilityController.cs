using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Safemeet.Models;
using Safemeet.Services;

namespace Safemeet.Api.Controllers
{
    [ApiController]
    [Route("api/Availability")]
    public class AvailabilityController : ControllerBase
    {
        private readonly IMongoCollection<AvailabilitySlot> _availabilitySlots;

        public AvailabilityController(MongoDbService mongoDbService)
        {
            _availabilitySlots = mongoDbService.GetCollection<AvailabilitySlot>("AvailabilitySlots");
        }

        [HttpPost]
        public async Task<IActionResult> Create(AvailabilitySlot slot)
        {
            await _availabilitySlots.InsertOneAsync(slot);
            return CreatedAtAction(nameof(GetById), new { id = slot.Id }, slot);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var slots = await _availabilitySlots.Find(_ => true).ToListAsync();
            return Ok(slots);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var slot = await _availabilitySlots.Find(s => s.Id == id).FirstOrDefaultAsync();
            if (slot == null)
            {
                return NotFound();
            }
            return Ok(slot);
        }

        [HttpGet("user/{userid}")]
        public async Task<IActionResult> GetByUser(string userid)
        {
            var slots = await _availabilitySlots.Find(s => s.UserId == userid).ToListAsync();
            return Ok(slots);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, AvailabilitySlot slot)
        {
            var result = await _availabilitySlots.ReplaceOneAsync(s => s.Id == id, slot);
            if (result.MatchedCount == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _availabilitySlots.DeleteOneAsync(s => s.Id == id);
            if (result.DeletedCount == 0) return NotFound();
            return NoContent();
        }
    }
}
