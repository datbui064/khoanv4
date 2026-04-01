using System.Net.Http.Json;
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

        // Tải file Excel mẫu
        public async Task<byte[]> DownloadTemplateAsync()
        {
            var response = await _http.GetAsync("api/Quizzes/template");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
        // Lấy danh sách câu hỏi theo chuyên mục cho Admin
        public async Task<List<QuestionDto>> GetQuestionsByCategoryAsync(int categoryId)
        {
            return await _http.GetFromJsonAsync<List<QuestionDto>>($"api/Quizzes/category/{categoryId}") ?? new();
        }
        // Cập nhật câu hỏi
        public async Task<bool> UpdateQuestionAsync(int id, QuestionDto question)
        {
            var response = await _http.PutAsJsonAsync($"api/Quizzes/question/{id}", question);
            return response.IsSuccessStatusCode;
        }

        // Xóa câu hỏi
        public async Task<bool> DeleteQuestionAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Quizzes/question/{id}");
            return response.IsSuccessStatusCode;
        }
        // Lấy đề thi ngẫu nhiên
        public async Task<List<QuestionDto>> GetRandomQuestionsAsync(int categoryId)
        {
            return await _http.GetFromJsonAsync<List<QuestionDto>>($"api/Quizzes/random/{categoryId}") ?? new();
        }

        // Nộp bài và nhận điểm
        // Nộp bài và nhận điểm
        public async Task<QuizResultDto?> SubmitQuizAsync(SubmitQuizRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/Quizzes/submit", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<QuizResultDto>();
            }
            return null;
        }

        // Lấy lịch sử cho Admin
        public async Task<List<dynamic>> GetHistoryAsync()
        {
            return await _http.GetFromJsonAsync<List<dynamic>>("api/Quizzes/history") ?? new();
        }
        // Import Excel (Đẩy file lên API)
        public async Task<string> ImportExcelAsync(int categoryId, MultipartFormDataContent content)
        {
            var response = await _http.PostAsync($"api/Quizzes/import/{categoryId}", content);
            if (response.IsSuccessStatusCode) return "Success";

            // Nếu lỗi, đọc thông báo lỗi từ API (VD: Lỗi ở dòng số X)
            return await response.Content.ReadAsStringAsync();
        }

        // Chỗ này sau này có thể thêm các hàm GetAllQuestions, GetSetting, UpdateSetting...
    }
}