using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using razor.Data;
using razor.Models;
using razor.Repository;

namespace razor.Interface
{
    public interface IProduct
    {
        Task<Product> CreateAsync(Product product);
        Task<Product?> DeleteAsync(int id);
        Task<Product> UpdateAsync(Product product);
        Task<List<Product>> ReadAsync();
    }
}