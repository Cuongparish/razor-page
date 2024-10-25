using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Npgsql;
using razor.Data;
using razor.Repository;
using razor.Service;
using razor.Middleware;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Serilog

var appInsightString = builder.Configuration["ApplicationInsights:InstrumentationKey"];
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = appInsightString;
});

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.ApplicationInsights(appInsightString, TelemetryConverter.Traces)
            .CreateLogger();
builder.Host.UseSerilog();

// Cấu hình dịch vụ cache cho session
builder.Services.AddDistributedMemoryCache(); // Cần thiết để sử dụng session

// Cấu hình session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian chờ của session
    options.Cookie.HttpOnly = true; // Cookie không thể truy cập từ JavaScript
    options.Cookie.IsEssential = true; // Cookie sẽ được lưu ngay cả khi người dùng không đồng ý cookie không cần thiết
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None; // Cần thiết cho các ứng dụng cần tương tác với OAuth
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Đảm bảo HTTPS
});


// Cấu hình JWT
var key = Encoding.ASCII.GetBytes("YourVeryStrongSecretKey1234567890"); // Thay thế bằng khóa bí mật của bạn
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Token hết hạn chính xác sau 30 phút
    };
})
.AddCookie("Cookies", options =>
{
    options.Cookie.HttpOnly = true;
    options.LoginPath = "/Login"; // Chuyển hướng đến trang đăng nhập khi không xác thực
})
.AddGoogle(options =>
{
    options.ClientId = "123";  // Thay thế bằng Google Client ID của bạn
    options.ClientSecret = "";  // Thay thế bằng Google Client Secret của bạn
    options.CallbackPath = "/signin-google";  // Đường dẫn trả về khi đăng nhập thành công
});

// Cấu hình DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProductRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts(); // HSTS cho môi trường sản xuất
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession(); // Sử dụng session

// Thêm middleware kiểm tra đăng nhập
app.UseMiddleware<AuthenticationMiddleware>(); // Đăng ký middleware kiểm tra đăng nhập ở đây

app.UseRouting();
app.UseAuthentication(); // Đảm bảo sử dụng authentication trước khi authorization
app.UseAuthorization();

// Chuyển hướng URL gốc đến trang đăng nhập
app.MapGet("/", async context =>
{
    context.Response.Redirect("/Login");
});

// Đăng ký các trang Razor
app.MapRazorPages();

app.Run();
