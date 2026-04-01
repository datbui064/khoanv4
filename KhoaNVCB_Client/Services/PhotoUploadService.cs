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

                // Giới hạn kích thước file upload (ở đây tôi để 5MB = 5 * 1024 * 1024 bytes)
                var fileContent = new StreamContent(file.OpenReadStream(5242880));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                // "file" chính là tên biến mà tham số API PhotosController đang chờ
                content.Add(fileContent, "file", file.Name);

                var response = await _httpClient.PostAsync("api/Photos", content);

                if (response.IsSuccessStatusCode)
                {
                    // Lấy kết quả trả về từ Backend (gồm có url và publicId)
                    var result = await response.Content.ReadFromJsonAsync<PhotoUploadResult>();
                    return result?.url; // Trả về link ảnh Cloudinary xịn xò
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi upload ảnh: {ex.Message}");
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