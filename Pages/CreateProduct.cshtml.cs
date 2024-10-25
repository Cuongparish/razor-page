using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using razor.Data;
using razor.Models;
using razor.Repository;

namespace razor.Pages
{
    public class CreateProduct : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductRepository _productRepository;

        [BindProperty]
        public Product Product { get; set; } = new Product();

        public CreateProduct(ApplicationDbContext context, ProductRepository productRepository)
        {
            _context = context;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            await _productRepository.CreateAsync(Product);

            return RedirectToPage("/Index");
        }
    }
}