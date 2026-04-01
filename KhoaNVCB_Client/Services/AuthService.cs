using System.Net.Http.Json;
using Blazored.LocalStorage;
using KhoaNVCB_Client.Models;
using Microsoft.AspNetCore.Components.Authorization;
namespace KhoaNVCB_Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;
        public AuthService(HttpClient http, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _http = http;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }
        public async Task<bool> Register(RegisterDto registerDto)
        {
            var response = await _http.PostAsJsonAsync("api/Auth/register", registerDto);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> Login(LoginDto loginDto)
        {
            var response = await _http.PostAsJsonAsync("api/Auth/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                if (result != null)
                {
                    await _localStorage.SetItemAsync("authToken", result.Token);
                    await _localStorage.SetItemAsync("fullName", result.FullName);
                    await _localStorage.SetItemAsync("role", result.Role);
                    await _localStorage.SetItemAsync("userId", result.AccountId.ToString()); // MỚI THÊM: Lưu ID người dùng
                    ((CustomAuthStateProvider)_authStateProvider).NotifyAuthState();
                    return true;
                }
            }
            return false;
        }

        public async Task Logout()
        {
            await _localStorage.ClearAsync();
            ((CustomAuthStateProvider)_authStateProvider).NotifyAuthState();
        }
    }
}