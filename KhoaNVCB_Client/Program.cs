using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using KhoaNVCB_Client;
using Blazored.LocalStorage;
using KhoaNVCB_Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. Đăng ký thư viện LocalStorage (Phải có cái này trước)
builder.Services.AddBlazoredLocalStorage();

// 2. Đọc cấu hình URL
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7240/";

// 3. Đăng ký Người gác cổng (Handler)
builder.Services.AddTransient<JwtAuthenticationHandler>();

// 4. Cấu hình "Đường ống" HttpClient xịn (Có gắn người gác cổng)
builder.Services.AddHttpClient("KhoaNVCB_API", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<JwtAuthenticationHandler>();

// 5. Đăng ký HttpClient mặc định CHỈ MỘT LẦN DUY NHẤT (Lấy từ đường ống xịn ở trên)
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("KhoaNVCB_API"));

// 6. Đăng ký các Service nghiệp vụ
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddAuthorizationCore();
// Đăng ký bảo vệ Custom của mình
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<KhoaNVCB_Client.Services.PhotoUploadService>();
builder.Services.AddScoped<QuizService>();
builder.Services.AddScoped<ScraperService>();

await builder.Build().RunAsync();