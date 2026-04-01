using System.Net.Http.Json;
using KhoaNVCB_Client.Models;

namespace KhoaNVCB_Client.Services
{
    public class CategoryService
    {
        private readonly HttpClient _http;

        public CategoryService(HttpClient http)
        {
            _http = http;
        }

        // Lấy danh sách danh mục (Công khai)
        public async Task<List<CategoryDto>> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<CategoryDto>>("api/Categories") ?? new();
        }

        // Thêm mới danh mục 
        public async Task<bool> CreateAsync(CategoryDto category)
        {
            var response = await _http.PostAsJsonAsync("api/Categories", category);
            return response.IsSuccessStatusCode;
        }

        // Cập nhật danh mục (Sửa) - VỪA THÊM
        public async Task<bool> UpdateAsync(int id, CategoryDto category)
        {
            var response = await _http.PutAsJsonAsync($"api/Categories/{id}", category);
            return response.IsSuccessStatusCode;
        }

        // Xóa danh mục
        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Categories/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}