using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // Để sử dụng HttpContext
using Microsoft.AspNetCore.Http.Extensions; // Để sử dụng GetString
using System.IdentityModel.Tokens.Jwt; // JwtSecurityTokenHandler
using Microsoft.IdentityModel.Tokens;   // SymmetricSecurityKey, TokenValidationParameters
using System.Text;                      // Encoding

namespace razor.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            // Kiểm tra token trong session
            // var token = context.Session.GetString("JWToken");
            string? token = context.Request.Cookies["JWToken"];
            bool isLoggedIn = !string.IsNullOrEmpty(token);

            // Nếu có token, kiểm tra tính hợp lệ của nó
            if (isLoggedIn)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                try
                {
                    var key = Encoding.ASCII.GetBytes("YourVeryStrongSecretKey1234567890");

                    // Thực hiện xác thực token
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,  // Kiểm tra token hết hạn
                        ClockSkew = TimeSpan.Zero // Token hết hạn chính xác
                    }, out SecurityToken validatedToken);

                    // Token hợp lệ, người dùng đã đăng nhập
                    isLoggedIn = true;
                }
                catch (SecurityTokenExpiredException)
                {
                    // Token đã hết hạn, cần xử lý
                    isLoggedIn = false;

                    // Xóa token khỏi session vì nó đã hết hạn
                    // context.Session.Remove("JWToken");
                    context.Response.Cookies.Delete("JWToken");

                    // Chuyển hướng người dùng đến trang đăng nhập
                    context.Response.Redirect("/Login");
                    return;
                }
                catch (Exception)
                {
                    // Token không hợp lệ
                    isLoggedIn = false;
                }
            }

            // Nếu chưa đăng nhập và không phải trang Login hoặc SignUp, chuyển hướng đến trang Login
            if (!isLoggedIn && context.Request.Path != "/Login" && context.Request.Path != "/SignUp" && context.Request.Path != "/signin-google")
            {
                context.Response.Redirect("/Login");
                return;
            }

            // Nếu đã đăng nhập và đang cố truy cập trang Login hoặc SignUp, chuyển hướng về trang chính
            if (isLoggedIn && (context.Request.Path == "/Login" || context.Request.Path == "/SignUp" || context.Request.Path == "/signin-google"))
            {
                context.Response.Redirect("/Index"); // Thay "/Index" bằng đường dẫn trang chính của bạn
                return;
            }

            await _next(context);
        }

    }
}