using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace KhoaNVCB_Client.Services
{
    public class JwtAuthenticationHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public JwtAuthenticationHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 1. Lấy token từ "két sắt" LocalStorage
            var token = await _localStorage.GetItemAsync<string>("authToken");

            // 2. Nếu có token, tự động gắn vào Header
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // 3. Cho phép request tiếp tục đi tới Server
            return await base.SendAsync(request, cancellationToken);
        }
    }
}