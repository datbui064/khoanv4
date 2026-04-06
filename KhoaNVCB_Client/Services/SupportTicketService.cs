using System.Net.Http.Json;
using KhoaNVCB_Client.Models;

namespace KhoaNVCB_Client.Services
{
    public class SupportTicketService
    {
        private readonly HttpClient _http;
        public SupportTicketService(HttpClient http) { _http = http; }

        public async Task<List<SupportTicketDto>?> GetAllTicketsAsync()
        {
            return await _http.GetFromJsonAsync<List<SupportTicketDto>>("api/chatbot/tickets");
        }

        public async Task<bool> ResolveTicketAsync(int id)
        {
            var response = await _http.PutAsync($"api/chatbot/ticket/{id}/resolve", null);
            return response.IsSuccessStatusCode;
        }
    }
}