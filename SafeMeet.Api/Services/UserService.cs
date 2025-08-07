using MongoDB.Driver;
using SafeMeet.Api.Models;
using System.Security.Claims;

namespace SafeMeet.Api.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(MongoDbService mongoDbService)
        {
            _users = mongoDbService.GetCollection<User>("Users");
        }

        public async Task<User> GetOrCreateUserAsync(ClaimsPrincipal principal)
        {
            try
            {
                var email = principal.FindFirst(ClaimTypes.Email)?.Value;
                var name = principal.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(email)) throw new Exception("Email not provided by OAuth provider");

                var user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

                if (user == null)
                {
                    user = new User
                    {
                        Email = email,
                        Name = name ?? "Unknown",
                        AuthProvider = "Google"
                    };

                    await _users.InsertOneAsync(user);
                }
                return user;
            }
            catch (TimeoutException ex)
            {
                throw new Exception("Database connection timeout. Please check your MongoDB connection.", ex);
            }
            catch (MongoAuthenticationException ex)
            {
                throw new Exception("MongoDB authentication failed. Please check your credentials.", ex);
            }
            catch (MongoConnectionException ex)
            {
                throw new Exception("Failed to connect to MongoDB. Please check your connection string and network connectivity.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while accessing the database: {ex.Message}", ex);
            }
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }
    }
}