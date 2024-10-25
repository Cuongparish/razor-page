using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using razor.Data;
using razor.Models;
using razor.Repository;


namespace razor.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ProductRepository _productRepository;

    public IndexModel(ApplicationDbContext context, ProductRepository productRepository)
    {
        _context = context;
        _productRepository = productRepository;
    }

    public List<Product> Products { get; set; } = new List<Product>();

    public async Task OnGetAsync()
    {
        // Products = await (_context.Products?.OrderBy(p => p.Id).ToListAsync() ?? Task.FromResult(new List<Product>()));
        Products = await _productRepository.ReadAsync();
    }

    public async Task<IActionResult> OnPostDelete(int id)
    {

        await _productRepository.DeleteAsync(id);
        // var product = await _context.Products.FindAsync(id);
        // if (product != null)
        // {

        //     _context.Products.Remove(product);
        //     await _context.SaveChangesAsync();
        // }
        return RedirectToPage();
    }


}
