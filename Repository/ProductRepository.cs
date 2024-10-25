using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using razor.Data;
using Microsoft.EntityFrameworkCore;
using razor.Interface;
using razor.Models;

namespace razor.Repository
{
    public class ProductRepository : IProduct
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> ReadAsync()
        {
            return await (_context.Products?.OrderBy(p => p.Id).ToListAsync() ?? Task.FromResult(new List<Product>()));

        }
        public async Task<Product> CreateAsync(Product product)
        {
            if (_context.Products != null)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
            }
            return product;
        }

        public async Task<Product?> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {

            _context.Attach(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return product;
        }


    }
}