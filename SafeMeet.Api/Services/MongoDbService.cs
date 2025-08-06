using MongoDB.Driver

namespace Safemeet.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["Mongo:ConnectionString"]);
            _database = client.GetDatabase(configuration.GetSection("Mongo:DatabaseName").Value);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
        
        
    }
}