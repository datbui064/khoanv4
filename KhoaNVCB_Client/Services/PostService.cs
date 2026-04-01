using System.Net.Http.Json;
using KhoaNVCB_Client.Models;

namespace KhoaNVCB_Client.Services
{
    public class PostService
    {
        private readonly HttpClient _http;

        public PostService(HttpClient http)
        {
            _http = http;
        }

        // Lấy toàn bộ bài viết
        public async Task<List<PostDto>> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<PostDto>>("api/Posts") ?? new();
        }

        // Lấy chi tiết bài viết theo ID
        public async Task<PostDto?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<PostDto>($"api/Posts/{id}");
        }

        // Tạo bài viết mới (Sẽ tự động đính kèm Token Admin)
        public async Task<bool> CreateAsync(PostDto post)
        {
            post.CreatedDate = DateTime.Now;
            var response = await _http.PostAsJsonAsync("api/Posts", post);
            return response.IsSuccessStatusCode;
        }

        // Xóa bài viết
        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Posts/{id}");
            return response.IsSuccessStatusCode;
        }
        // Cập nhật bài viết (Fix lỗi CS1061)
        public async Task<bool> UpdateAsync(int id, PostDto post)
        {
            var response = await _http.PutAsJsonAsync($"api/Posts/{id}", post);
            return response.IsSuccessStatusCode;
        }

        // Đẩy file Word lên API để bóc chữ
        public async Task<string?> ExtractWordAsync(Microsoft.AspNetCore.Components.Forms.IBrowserFile file)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(file.OpenReadStream(10485760)); // Giới hạn file 10MB
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "file", file.Name);

                var response = await _http.PostAsync("api/Posts/extract-word", content);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<WordExtractionResult>();
                    return result?.text;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi gửi file Word: " + ex.Message);
                return null;
            }
        }
        // Lấy danh sách bình luận của 1 bài viết
        public async Task<List<CommentDto>> GetCommentsByPostAsync(int postId)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<CommentDto>>($"api/Comments/post/{postId}") ?? new();
            }
            catch
            {
                return new List<CommentDto>();
            }
        }

        // Gửi bình luận mới
        public async Task<bool> CreateCommentAsync(CreateCommentDto comment)
        {
            var response = await _http.PostAsJsonAsync("api/Comments", comment);
            return response.IsSuccessStatusCode;
        }
    }
    public class WordExtractionResult
    {
        public string text { get; set; } = "";
    }
}