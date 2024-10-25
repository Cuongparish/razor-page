using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace razor.Pages
{
    public class Logout : PageModel
    {
        public IActionResult OnPostAsync()
        {
            // Xóa token trong cookies
            HttpContext.Response.Cookies.Delete("JWToken");

            // Chuyển hướng về trang đăng nhập
            return RedirectToPage("/Login");
        }
    }
}