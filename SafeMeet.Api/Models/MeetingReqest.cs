using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Safemeet.Models
{
    public class MeetingRequest{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id {get; set;}

        public required string CreatedBy {get; set;}

        public required string Title {get; set;}

        public required string Agenda {get; set;}

        public required List<string> Attendees {get; set;}

        public DateTime PreferredDateStart {get; set;}

        public DateTime PreferredDateEnd {get; set;}

        public TimeSpan PreferredTimeStart {get; set;}

        public TimeSpan PreferredTimeEnd {get; set;}

        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    }
}