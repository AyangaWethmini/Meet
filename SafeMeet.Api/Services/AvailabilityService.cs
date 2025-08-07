using MongoDB.Driver;
using SafeMeet.Api.Models;

namespace SafeMeet.Api.Services
{
    public class AvailabilityService
    {
        private readonly IMongoCollection<AvailabilitySlot> _availabilitySlots;

        public AvailabilityService(MongoDbService mongoDbService)
        {
            _availabilitySlots = mongoDbService.GetCollection<AvailabilitySlot>("AvailabilitySlots");
        }

        public async Task<List<AvailabilitySlot>> GetAvailabilityByUserIdAsync(string userId)
        {
            return await _availabilitySlots.Find(slot => slot.UserId == userId).ToListAsync();
        }

        public async Task<AvailabilitySlot> CreateAvailabilitySlotAsync(AvailabilitySlot slot)
        {
            await _availabilitySlots.InsertOneAsync(slot);
            return slot;
        }

        public async Task<List<AvailabilitySlot>> CreateMultipleAvailabilitySlotsAsync(List<AvailabilitySlot> slots)
        {
            await _availabilitySlots.InsertManyAsync(slots);
            return slots;
        }

        public async Task<bool> UpdateAvailabilitySlotAsync(string id, AvailabilitySlot updatedSlot)
        {
            var result = await _availabilitySlots.ReplaceOneAsync(
                slot => slot.Id == id,
                updatedSlot
            );
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateAvailabilitySlotAsync(string id, string userId, AvailabilitySlot updatedSlot)
        {
            updatedSlot.Id = id;
            updatedSlot.UserId = userId;
            var result = await _availabilitySlots.ReplaceOneAsync(
                slot => slot.Id == id && slot.UserId == userId,
                updatedSlot
            );
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAvailabilitySlotAsync(string id)
        {
            var result = await _availabilitySlots.DeleteOneAsync(slot => slot.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> DeleteAvailabilitySlotAsync(string id, string userId)
        {
            var result = await _availabilitySlots.DeleteOneAsync(slot => slot.Id == id && slot.UserId == userId);
            return result.DeletedCount > 0;
        }

        public async Task<List<AvailabilitySlot>> GetAvailabilityByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            return await _availabilitySlots.Find(slot =>
                slot.UserId == userId &&
                slot.Date >= startDate &&
                slot.Date <= endDate
            ).ToListAsync();
        }

        public async Task<AvailabilitySlot?> GetAvailabilitySlotByIdAsync(string id, string userId)
        {
            return await _availabilitySlots.Find(slot => slot.Id == id && slot.UserId == userId).FirstOrDefaultAsync();
        }
    }
}
