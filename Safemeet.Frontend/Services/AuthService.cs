
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using SafeMeet.Frontend.Models;
using System.Net.Http.Json;

namespace SafeMeet.Frontend.Services

{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private string _token;
        private User _currentUser;


        public AuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SafeMeetApi");

        }

        public async Task<User> LoginAsync()
        {

            // Redirect to API login endpoint
            // This will be handled by a button click in the UI
            return null; // Placeholder, actual login handled via redirect

        }

        public void SetToken(string token, User user)
        {
            _token = token;
            _currentUser = user;

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        }

        public User CurrentUser => _currentUser;
        public bool IsAuthenticated => !string.IsNullOrEmpty(_token);
    }
}