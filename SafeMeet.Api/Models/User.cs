using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SafeMeet.Api.Models
{
    public class User 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id {get; set;}

        [BsonElement("name")]
        public required string Name {get; set;}

        [BsonElement("email")]
        public required string Email {get; set;}

        public required string AuthProvider {get; set;}
    }
}
