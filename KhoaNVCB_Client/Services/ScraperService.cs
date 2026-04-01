using System.Net.Http.Json;
using KhoaNVCB_Client.Models;

namespace KhoaNVCB_Client.Services
{
    public class ScraperService
    {
        private readonly HttpClient _http;

        public ScraperService(HttpClient http)
        {
            _http = http;
        }

        // Gọi Bot đi cào tin VnExpress
        public async Task<string> RunVnExpressScraperAsync()
        {
            var response = await _http.PostAsync("api/Scraper/run-vnexpress", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return result?.GetProperty("message").GetString() ?? "Cào tin thành công!";
            }
            return "Lỗi khi khởi chạy Bot.";
        }
        public async Task<string> RunCandScraperAsync()
        {
            var response = await _http.PostAsync("api/Scraper/run-cand", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return result?.GetProperty("message").GetString() ?? "Cào tin CAND thành công!";
            }
            return "Lỗi khi gọi Bot CAND.";
        }
        public async Task<string> RunTuoiTreScraperAsync()
        {
            var response = await _http.PostAsync("api/Scraper/run-tuoitre", null);
            return response.IsSuccessStatusCode
                ? (await response.Content.ReadFromJsonAsync<dynamic>())?.GetProperty("message").GetString() ?? "Thành công"
                : "Lỗi Bot Tuổi Trẻ.";
        }

        public async Task<string> RunThanhNienScraperAsync()
        {
            var response = await _http.PostAsync("api/Scraper/run-thanhnien", null);
            return response.IsSuccessStatusCode
                ? (await response.Content.ReadFromJsonAsync<dynamic>())?.GetProperty("message").GetString() ?? "Thành công"
                : "Lỗi Bot Thanh Niên.";
        }
        // Gọi Bot cào 5 kênh YouTube
        public async Task<string> RunYouTubeScraperAsync()
        {
            var response = await _http.PostAsync("api/Scraper/run-youtube", null);
            return response.IsSuccessStatusCode
                ? (await response.Content.ReadFromJsonAsync<dynamic>())?.GetProperty("message").GetString() ?? "Thành công"
                : "Lỗi Bot YouTube.";
        }
        // Lấy danh sách tin đang chờ duyệt
        public async Task<List<PostDto>> GetPendingPostsAsync()
        {
            return await _http.GetFromJsonAsync<List<PostDto>>("api/Scraper/pending") ?? new();
        }
    }
}