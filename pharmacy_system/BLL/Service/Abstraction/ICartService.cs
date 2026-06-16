using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service.Abstraction
{
    public interface ICartService
    {
        Task AddToCartAsync(
           string userId,
           CreateCartItemDto dto);

        Task<CartDto?> GetCartAsync(string userId);

        Task RemoveFromCartAsync(
            string userId,
            int productId);
    }
}
