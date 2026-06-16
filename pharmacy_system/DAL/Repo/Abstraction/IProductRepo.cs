using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repo.Abstraction
{
    public interface IProductRepo : IGenericRepo<Product>
    {
        Task<IEnumerable<Product>> GetProductsWithCategoryAsync();

        Task<Product?> GetProductWithCategoryAsync(int id);
        Task<Category?> GetCategoryByIdAsync(int id);
    }
}
