using SafeMeet.Frontend.Models;
using System.Net.Http.Json;


namespace SafeMeet.Frontend.Services


{
    public class AvailabilityService
    {
        private readonly HttpClient _httpClient;


        public AvailabilityService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SafeMeetApi");

        }

        public async Task<List<AvailabilitySlot>> GetUserSlotsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<AvailabilitySlot>>("Availability/user");
        }

        public async Task<AvailabilitySlot> CreateSlotAsync(AvailabilitySlot slot)
        {
            var response = await _httpClient.PostAsJsonAsync("Availability", slot);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AvailabilitySlot>();
        }

        public async Task UpdateSlotAsync(string id, AvailabilitySlot slot)
        {
            var response = await _httpClient.PutAsJsonAsync($"Availability/{id}", slot);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteSlotAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"Availability/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}