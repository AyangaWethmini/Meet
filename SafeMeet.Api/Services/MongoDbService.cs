using MongoDB.Driver;

namespace SafeMeet.Api.Services
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

            // Trim quotes and spaces if present
            connectionString = connectionString?.Trim().Trim('"', '\'');
            databaseName = databaseName?.Trim().Trim('"', '\'');

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("MongoDB connection string is not configured. Please set MONGO_CONNECTION_STRING environment variable.");
            }

            if (string.IsNullOrEmpty(databaseName))
            {
                throw new InvalidOperationException("MongoDB database name is not configured. Please set MONGO_DATABASE_NAME environment variable.");
            }

            try
            {
                // Configure TLS/SSL settings for Windows compatibility
                var clientSettings = MongoClientSettings.FromConnectionString(connectionString);

                // Configure timeouts
                clientSettings.ConnectTimeout = TimeSpan.FromSeconds(30);
                clientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(30);
                clientSettings.SocketTimeout = TimeSpan.FromSeconds(30);

                // TLS Configuration for Windows
                clientSettings.UseTls = true;
                clientSettings.AllowInsecureTls = false;

                // Configure TLS settings for better Windows compatibility
                clientSettings.SslSettings = new SslSettings
                {
                    EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12,
                    CheckCertificateRevocation = false
                };

                var client = new MongoClient(clientSettings);
                _database = client.GetDatabase(databaseName);

                Console.WriteLine($"MongoDB client configured for database: {databaseName}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"MongoDB connection error: {ex.Message}");
                throw new InvalidOperationException($"Failed to connect to MongoDB: {ex.Message}", ex);
            }
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}