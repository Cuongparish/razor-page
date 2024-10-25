using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using razor.Data;
using razor.Models;
using razor.Repository;
using razor.Service;

namespace razor.Pages
{
    public class Login : PageModel
    {

        // Class to store input data
        private readonly ApplicationDbContext _context;
        private readonly ProductRepository _productRepository;
        private readonly AuthService _authService;
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public User User { get; set; } = new User();
        public Product Product { get; set; } = new Product();

        public Login(ILogger<IndexModel> logger, ApplicationDbContext context, ProductRepository productRepository, AuthService authService)
        {
            _context = context;
            _productRepository = productRepository;
            _authService = authService;
            _logger = logger;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == User.Username);

            if (user == null || user.IsLocked)
            {
                TempData["ErrorMessage"] = "Tài khoản không tồn tại hoặc đã bị khóa.";
                return RedirectToPage("/Login");
            }

            if (!VerifyPassword(User.PasswordHash, user.PasswordHash))
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= 5)
                {
                    user.IsLocked = true;
                }
                await _context.SaveChangesAsync();
                TempData["ErrorMessage"] = "Mật khẩu sai. Tài khoản sẽ bị khóa sau 5 lần.";
                return RedirectToPage("/Login");
            }

            // Reset số lần đăng nhập thất bại nếu đăng nhập thành công
            user.FailedLoginAttempts = 0;
            await _context.SaveChangesAsync();

            // Tạo token
            var token = _authService.GenerateJwtToken(User.Username);
            // HttpContext.Session.SetString("JWToken", token);
            // Lưu token vào cookie với thời gian sống 30 phút
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,  // Giúp ngăn JavaScript truy cập vào cookie
                Secure = true,    // Chỉ cho phép khi giao thức HTTPS
                Expires = DateTime.UtcNow.AddMinutes(30)  // Thiết lập thời gian sống của cookie
            };

            HttpContext.Response.Cookies.Append("JWToken", token, cookieOptions);

            _logger.LogInformation("Đăng nhập thành công cho người dùng: {Username}", User.Username);

            // Redirect đến trang Index
            return RedirectToPage("/Index");
        }


        private bool VerifyPassword(string password, string storedHash)
        {
            // Thực hiện kiểm tra mật khẩu (ví dụ: mã hóa hash và so sánh)
            return password == storedHash;  // Ví dụ đơn giản, thay thế bằng mã hóa mật khẩu thực sự
        }
    }

}