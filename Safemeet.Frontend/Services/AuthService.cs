using SafeMeet.Frontend.Models;
using System.Net.Http.Json;

namespace Safemeet.Frontend.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private string _token;
        private User _currentUser;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<User> LoginAsync()
        {
            return null; //implement the correct logic to redirect to backend api 
        }

        public void SetToken(string token, User user)
        {
            _token = token;
            _currentUser = user;
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public User CurrentUser => _currentUser;
        public bool IsAuthenticated => !string.IsNullOrEmpty(_token);
    }
}