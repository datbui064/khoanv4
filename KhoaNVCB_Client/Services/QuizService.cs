using System.Net.Http.Json;
using System.Text.Json;
using KhoaNVCB_Client.Models;

namespace KhoaNVCB_Client.Services
{
    public class QuizService
    {
        private readonly HttpClient _http;

        public QuizService(HttpClient http)
        {
            _http = http;
        }

        // --- CÁC HÀM CŨ GIỮ NGUYÊN ---
        public async Task<byte[]> DownloadTemplateAsync()
        {
            var response = await _http.GetAsync("api/Quizzes/template");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<List<QuestionDto>> GetQuestionsByCategoryAsync(int categoryId)
        {
            return await _http.GetFromJsonAsync<List<QuestionDto>>($"api/Quizzes/category/{categoryId}") ?? new();
        }

        public async Task<bool> UpdateQuestionAsync(int id, QuestionDto question)
        {
            var response = await _http.PutAsJsonAsync($"api/Quizzes/question/{id}", question);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteQuestionAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Quizzes/question/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<string> ImportExcelAsync(int categoryId, MultipartFormDataContent content)
        {
            var response = await _http.PostAsync($"api/Quizzes/import/{categoryId}", content);
            if (response.IsSuccessStatusCode) return "Success";
            return await response.Content.ReadAsStringAsync();
        }

        // Lấy danh sách câu hỏi ngẫu nhiên theo chuyên đề (hoặc tất cả nếu categoryId = 0)
        public async Task<List<QuestionDto>> GetRandomQuestionsAsync(int categoryId)
        {
            try
            {
                // Gọi đến endpoint API tương ứng (giả sử là api/Quizzes/random/{categoryId})
                var response = await _http.GetAsync($"api/Quizzes/random/{categoryId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<QuestionDto>>() ?? new();
                }
                return new List<QuestionDto>();
            }
            catch
            {
                return new List<QuestionDto>();
            }
        }

        // ===============================================
        // --- CÁC HÀM MỚI: QUẢN LÝ PHIÊN THI (SESSION) ---
        // ===============================================

        // ===============================================
        // --- CÁC HÀM MỚI: QUẢN LÝ PHIÊN THI (SESSION) ---
        // ===============================================

        public async Task<JsonElement?> CreateSessionAsync(object request)
        {
            var response = await _http.PostAsJsonAsync("api/Quizzes/session", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<JsonElement>();
            }
            return null;
        }

        public async Task<List<JsonElement>> GetAllSessionsAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<JsonElement>>("api/Quizzes/sessions") ?? new();
            }
            catch { return new List<JsonElement>(); }
        }

        public async Task<bool> ToggleSessionAsync(int id)
        {
            var response = await _http.PutAsync($"api/Quizzes/session/{id}/toggle", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<JsonElement>> GetSessionHistoryAsync(int sessionId)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<JsonElement>>($"api/Quizzes/session/{sessionId}/history") ?? new();
            }
            catch { return new List<JsonElement>(); }
        }

        // Học viên lấy đề thông qua mã SessionCode
        public async Task<JsonElement?> JoinSessionAsync(string sessionCode)
        {
            var response = await _http.GetAsync($"api/Quizzes/join/{sessionCode}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<JsonElement>();
            }
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        // ===============================================
        // --- HÀM THI DÀNH CHO SINH VIÊN ---
        // ===============================================

        // Học viên lấy đề thông qua mã SessionCode
        public async Task<bool> DeleteSessionAsync(int sessionId)
        {
            var response = await _http.DeleteAsync($"api/Quizzes/session/{sessionId}");
            return response.IsSuccessStatusCode;
        }
        public async Task<QuizResultDto?> SubmitQuizAsync(SubmitQuizRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/Quizzes/submit", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<QuizResultDto>();
            }
            return null;
        }
    }
}