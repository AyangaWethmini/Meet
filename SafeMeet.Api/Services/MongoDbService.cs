using MongoDB.Driver;

namespace Safemeet.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration configuration)
        {
            // Read from environment variables first, then fall back to configuration
            var connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") 
                ?? configuration["Mongo:ConnectionString"];
            var databaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME") 
                ?? configuration["Mongo:DatabaseName"];

            // Trim quotes if present
            connectionString = connectionString?.Trim('"', '\'');
            databaseName = databaseName?.Trim('"', '\'');

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("MongoDB connection string is not configured. Please set MONGO_CONNECTION_STRING environment variable.");
            }

            if (string.IsNullOrEmpty(databaseName))
            {
                throw new InvalidOperationException("MongoDB database name is not configured. Please set MONGO_DATABASE_NAME environment variable.");
            }

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
        
        
   }
}