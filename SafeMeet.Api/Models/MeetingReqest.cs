using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SafeMeet.Api.Models
{
    public class MeetingRequest{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id {get; set;}

        public string CreatedBy {get; set;}

        public  string Title {get; set;}

        public string Agenda {get; set;}

        public List<string> Attendees {get; set;}

        public DateTime PreferredDateStart {get; set;}

        public DateTime PreferredDateEnd {get; set;}

        public TimeSpan PreferredTimeStart {get; set;}

        public TimeSpan PreferredTimeEnd {get; set;}

        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    }
}