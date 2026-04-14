using System.Net.Http.Headers;
using System.Net.Http.Json;
using KhoaNVCB_Client.Models;
using Microsoft.JSInterop;

namespace KhoaNVCB_Client.Services
{
    public class PostService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;

        public PostService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        // HÀM HỖ TRỢ: Lấy token an toàn không bao giờ gây lỗi Null
        private async Task SetAuthorizeHeader()
        {
            try
            {
                // Lấy trực tiếp từ trình duyệt qua JS
                var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");

                if (!string.IsNullOrEmpty(token))
                {
                    // Xóa dấu ngoặc kép và gán vào Header
                    _http.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token.Trim('"'));
                }
                else
                {
                    _http.DefaultRequestHeaders.Authorization = null;
                }
            }
            catch
            {
                // Im lặng nếu lỗi (xảy ra khi render phía Server hoặc JS chưa sẵn sàng)
            }
        }
        public async Task<List<PostDto>> GetAllAsync()
        {
            try
            {
                // Không cần token nếu bạn để API này là AllowAnonymous ở Backend
                return await _http.GetFromJsonAsync<List<PostDto>>("api/Posts") ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi GetAllAsync: {ex.Message}");
                return new List<PostDto>();
            }
        }


        // Lấy chi tiết bài viết theo ID
        public async Task<PostDto?> GetByIdAsync(int id)
        {
            await SetAuthorizeHeader();
            try
            {
                return await _http.GetFromJsonAsync<PostDto>($"api/Posts/{id}");
            }
            catch { return null; }
        }

        // Lấy danh sách bài viết mới nhất (Dùng cho Sidebar)
        public async Task<List<PostDto>> GetRecentPublishedAsync(int count)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<PostDto>>($"api/Posts/recent/{count}") ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy bài viết mới: {ex.Message}");
                return new List<PostDto>();
            }
        }

        // Lấy danh sách bài viết cho Admin (Có phân trang)
        public async Task<PagedResultDto<PostListItemDto>?> GetAdminPostsPagedAsync(
            int page, int pageSize, string? searchTerm, string? categoryName, string sortBy, string? status = null)
        {
            try
            {
                await SetAuthorizeHeader(); // ĐÃ SỬA: Thay thế logic _localStorage cũ

                var url = $"api/Posts/admin-paged?page={page}&pageSize={pageSize}";
                if (!string.IsNullOrWhiteSpace(searchTerm)) url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
                if (!string.IsNullOrWhiteSpace(categoryName)) url += $"&categoryName={Uri.EscapeDataString(categoryName)}";
                if (!string.IsNullOrWhiteSpace(status)) url += $"&status={Uri.EscapeDataString(status)}";
                url += $"&sortBy={sortBy}";

                return await _http.GetFromJsonAsync<PagedResultDto<PostListItemDto>>(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy danh sách Admin: {ex.Message}");
                return null;
            }
        }

        // Lấy danh sách bài viết cho người dùng (Public)
        // Lấy danh sách bài viết cho người dùng (Public)
        public async Task<PagedResultDto<PostListItemDto>?> GetPublicPostsPagedAsync(
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? categoryName = null,
            string sortBy = "newest",
            string? sourceType = null,
            int? yearType = null) // Tham số đã có sẵn ở đây
        {
            try
            {
                var url = $"api/Posts/paged?page={page}&pageSize={pageSize}";

                if (!string.IsNullOrWhiteSpace(searchTerm))
                    url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";

                if (!string.IsNullOrWhiteSpace(categoryName))
                    url += $"&categoryName={Uri.EscapeDataString(categoryName)}";

                if (!string.IsNullOrWhiteSpace(sourceType))
                    url += $"&sourceType={Uri.EscapeDataString(sourceType)}";

                // --- DÒNG CẦN THÊM MỚI ---
                if (yearType.HasValue)
                    url += $"&yearType={yearType.Value}";

                url += $"&sortBy={sortBy}";

                return await _http.GetFromJsonAsync<PagedResultDto<PostListItemDto>>(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy bài viết Public: {ex.Message}");
                return null;
            }
        }
        // Cập nhật bài viết
        public async Task<bool> UpdateAsync(int id, PostDto post)
        {
            await SetAuthorizeHeader(); // ĐÃ SỬA: Không dùng _localStorage.Get nữa
            var response = await _http.PutAsJsonAsync($"api/Posts/{id}", post);
            return response.IsSuccessStatusCode;
        }

        // Tạo bài viết mới
        public async Task<bool> CreateAsync(PostDto post)
        {
            await SetAuthorizeHeader();
            post.CreatedDate = DateTime.Now;
            var response = await _http.PostAsJsonAsync("api/Posts", post);
            return response.IsSuccessStatusCode;
        }

        // Xóa bài viết
        public async Task<bool> DeleteAsync(int id)
        {
            await SetAuthorizeHeader();
            var response = await _http.DeleteAsync($"api/Posts/{id}");
            return response.IsSuccessStatusCode;
        }

        // Lấy bình luận
        public async Task<List<CommentDto>> GetCommentsByPostAsync(int postId)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<CommentDto>>($"api/Comments/post/{postId}") ?? new();
            }
            catch { return new List<CommentDto>(); }
        }

        // Gửi bình luận
        public async Task<bool> CreateCommentAsync(CreateCommentDto comment)
        {
            var response = await _http.PostAsJsonAsync("api/Comments", comment);
            return response.IsSuccessStatusCode;
        }

        // Trích xuất file Word
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
            catch { return null; }
        }
    }

    public class WordExtractionResult { public string text { get; set; } = ""; }
}