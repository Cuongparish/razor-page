using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using razor.Data;
using razor.Models;
using razor.Repository;
using System.Threading.Tasks;

namespace razor.Pages
{
    public class UpdateProductModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        private readonly ProductRepository _productRepository;

        public UpdateProductModel(ApplicationDbContext context, ProductRepository productRepository)
        {
            _context = context;
            _productRepository = productRepository;
        }

        [BindProperty]
        public Product Product { get; set; } = new Product();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            Product = existingProduct;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            await _productRepository.UpdateAsync(Product);

            return RedirectToPage("Index");
        }

    }
}
