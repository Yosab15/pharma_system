using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service.Abstraction
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();

        Task<ProductDto?> GetByIdAsync(int id);

        Task AddAsync(CreateProductDto dto);

        Task DeleteAsync(int id);
        Task UpdateAsync(int id, UpdateProductDto dto);
    }
}
