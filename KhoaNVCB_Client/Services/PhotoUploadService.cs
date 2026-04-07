using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace KhoaNVCB_Client.Services
{
    public class PhotoUploadService
    {
        private readonly HttpClient _httpClient;

        public PhotoUploadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Hàm này nhận file từ giao diện Blazor và đẩy lên Backend
        public async Task<string?> UploadPhotoAsync(IBrowserFile file)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                // Đọc stream
                var fileStream = file.OpenReadStream(10 * 1024 * 1024); // Tăng lên 10MB cho thoải mái
                var fileContent = new StreamContent(fileStream);

                // Cung cấp Content-Type cho từng phần tử file
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

                // Sử dụng tên field "file" khớp với tham số của Controller
                content.Add(fileContent, "file", file.Name);

                var response = await _httpClient.PostAsync("api/Photos", content);

                if (response.IsSuccessStatusCode)
                {
                    // Cẩn thận: Backend trả về { "location": "..." }
                    var jsonNode = await response.Content.ReadFromJsonAsync<System.Text.Json.Nodes.JsonObject>();
                    return jsonNode?["location"]?.ToString();
                }

                var err = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Lỗi server: {err}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi ngoại lệ: {ex.Message}");
                return null;
            }
        }
    }

    // Class phụ để hứng dữ liệu JSON do Backend trả về
    public class PhotoUploadResult
    {
        public string url { get; set; } = "";
        public string publicId { get; set; } = "";
    }
}