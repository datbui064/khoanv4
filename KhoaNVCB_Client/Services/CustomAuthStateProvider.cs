using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace KhoaNVCB_Client.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                var role = await _localStorage.GetItemAsync<string>("role");
                var fullName = await _localStorage.GetItemAsync<string>("fullName");
                var userId = await _localStorage.GetItemAsync<string>("userId"); // LẤY ID RA

                if (string.IsNullOrWhiteSpace(token))
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, fullName ?? "User"),
                new Claim(ClaimTypes.Role, role ?? "Student"),
                new Claim(ClaimTypes.NameIdentifier, userId ?? "") // QUAN TRỌNG: Ghi ID vào Claim
            };

                var identity = new ClaimsIdentity(claims, "jwtAuth");
                var user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
            }
            catch
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        // Gọi hàm này để báo cho Blazor biết mỗi khi Login hoặc Logout
        public void NotifyAuthState()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}