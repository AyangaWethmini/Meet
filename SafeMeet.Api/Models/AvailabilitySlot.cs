using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Safemeet.Models
{
    public class AvailabilitySlot
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {get; set;}

        public string UserId {get; set;}

        public DateTime StartTime {get; set;}

        public DateTime EndTime {get; set;}

        public DateTime Date {get; set;} 

        [Range(1, 5)]
        public int PreferanceRateing {get; set;}
    }
}