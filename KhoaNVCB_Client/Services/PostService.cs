using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;
using KhoaNVCB_Client.Models;
using Hanssens.Net;

namespace KhoaNVCB_Client.Services
{
    public class PostService
    {
        private readonly HttpClient _http;
        private readonly LocalStorage _localStorage;

        public PostService(HttpClient http)
        {
            _http = http;
            var config = new LocalStorageConfiguration();
            _localStorage = new LocalStorage(config);
        }

        // --- HÀM MỚI THÊM VÀO ĐỂ SỬA LỖI ---
        // Lấy danh sách bài viết mới nhất đã xuất bản (Tối ưu cho Server)
        public async Task<List<PostDto>> GetRecentPublishedAsync(int count)
        {
            try
            {
                // Lưu ý: Endpoint "api/Posts/recent/{count}" cần được định nghĩa ở Controller phía Backend
                return await _http.GetFromJsonAsync<List<PostDto>>($"api/Posts/recent/{count}") ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy bài viết mới: {ex.Message}");
                return new List<PostDto>();
            }
        }

        // Lấy toàn bộ bài viết
        public async Task<List<PostDto>> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<PostDto>>("api/Posts") ?? new();
        }
        public async Task<(List<PostDto> Posts, int TotalPages)> GetPagedPostsAsync(int page, int pageSize, int? categoryId = null, string? sourceType = null)
        {
            var url = $"api/Posts/paged?page={page}&pageSize={pageSize}";
            if (categoryId.HasValue) url += $"&categoryId={categoryId}";
            if (!string.IsNullOrEmpty(sourceType)) url += $"&sourceType={sourceType}";

            var response = await _http.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var posts = await response.Content.ReadFromJsonAsync<List<PostDto>>() ?? new();
                int totalPages = 1;

                // SỬA TẠI ĐÂY: Dùng TryGetValues để tránh lỗi "Key not present"
                if (response.Headers.TryGetValues("X-Pagination", out var headerValues))
                {
                    var paginationHeader = headerValues.FirstOrDefault();
                    if (!string.IsNullOrEmpty(paginationHeader))
                    {
                        using var meta = System.Text.Json.JsonDocument.Parse(paginationHeader);
                        if (meta.RootElement.TryGetProperty("totalPages", out var prop))
                        {
                            totalPages = prop.GetInt32();
                        }
                    }
                }

                return (posts, totalPages);
            }

            return (new List<PostDto>(), 1);
        }  // Lấy chi tiết bài viết theo ID
        public async Task<PostDto?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<PostDto>($"api/Posts/{id}");
        }

        // Tạo bài viết mới
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

        // Cập nhật bài viết
        public async Task<bool> UpdateAsync(int id, PostDto post)
        {
            var token = _localStorage.Get<string>("authToken");

            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.Replace("\"", ""));
            }

            var response = await _http.PutAsJsonAsync($"api/Posts/{id}", post);
            return response.IsSuccessStatusCode;
        }

        // Đẩy file Word lên API để bóc chữ
        public async Task<string?> ExtractWordAsync(Microsoft.AspNetCore.Components.Forms.IBrowserFile file)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(file.OpenReadStream(10485760));
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
        // Thêm hàm này vào PostService.cs
        public async Task<List<PostListItemDto>> GetAdminPostsAsync()
        {
            try
            {
                // Gọi đến Endpoint đã sửa ở Backend (chỉ trả về list nhẹ)
                return await _http.GetFromJsonAsync<List<PostListItemDto>>("api/Posts") ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tải danh sách Admin: {ex.Message}");
                return new List<PostListItemDto>();
            }
        }
        // Lấy danh sách bình luận
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