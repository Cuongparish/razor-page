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
    public class SignUp : PageModel
    {
        // Class to store input data
        private readonly ApplicationDbContext _context;
        private readonly ProductRepository _productRepository;
        private readonly AuthService _authService;
        private readonly ILogger<IndexModel> _logger;


        [BindProperty]
        public User User { get; set; } = new User();
        public Product Product { get; set; } = new Product();

        public SignUp(ILogger<IndexModel> logger, ApplicationDbContext context, ProductRepository productRepository, AuthService authService)
        {
            _context = context;
            _productRepository = productRepository;
            _authService = authService;
            _logger = logger;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == User.Username);

            if (user != null)
            {

                return new ObjectResult(new { message = "Tài khoản đã tồn tại" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
            User.FailedLoginAttempts = 0;
            User.IsLocked = false;
            _context.Users.Add(User);
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
            _logger.LogInformation("Trang Index đã được truy cập.");
            // Redirect đến trang Index
            return RedirectToPage("/Index");
        }
    }
}