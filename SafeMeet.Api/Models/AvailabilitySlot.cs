using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SafeMeet.Api.Models
{
    public class AvailabilitySlot
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public required string UserId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime Date { get; set; }

        [Range(1, 5)]
        public int PreferenceRating { get; set; }
    }
}