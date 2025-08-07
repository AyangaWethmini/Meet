using MongoDb.Driver;
using SafeMeet.Api.Models;
using System.Security.Claims;

namespace Safemeet.Api.Services{
    public class UserService{
        private readonly IMongoCollection<User> _users;

        public UserService(MongoDbService mongoDbService) {
            _users = mongoDbService.GetCollection<User>("Users");
        }

        public async Task<User> GetOrCreateUserAsync(ClaimsPrinciple principle){
            var email = principle.FindFirst(ClaimTypes.Email)?.Value;
            var name = principle.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(email)) throw new Exeption("Email not provided by OAuth provider");

            var user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

            if(user == null) {
                user new User{
                    Email = email,
                    Name = name ?? "Unknown",
                    AuthProvider = "Google"
                };

                await _users.InsertOneAsync(user);
            }
            return user;
        }

        public async Task<User> GetUserByIdAsync(string id){
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }
    }
}