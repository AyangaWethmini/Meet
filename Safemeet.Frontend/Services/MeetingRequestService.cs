using SafeMeet.Frontend.Models;
using System.Net.Http.Json;

namespace SafeMeet.Frontend.Services
{
    public class MeetingRequestService
    {
        private readonly HttpClient _httpClient;

        public MeetingRequestService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SafeMeetApi");
        }

        public async Task<List<MeetingRequest>> GetUserRequestsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<MeetingRequest>>("MeetingRequest/my-requests");
        }

        public async Task<MeetingRequest> GetRequestByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<MeetingRequest>($"MeetingRequest/{id}");
        }

        public async Task<MeetingRequest> CreateRequestAsync(MeetingRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("MeetingRequest", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MeetingRequest>();
        }

        public async Task UpdateRequestAsync(string id, MeetingRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"MeetingRequest/{id}", request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteRequestAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"MeetingRequest/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}