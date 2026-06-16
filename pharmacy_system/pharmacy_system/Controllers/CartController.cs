using BLL.DTOs;
using BLL.Service.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace pharmacy_system.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // 🟢 ADD TO CART
        [HttpPost]
        public async Task<IActionResult> AddToCart(CreateCartItemDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _cartService.AddToCartAsync(userId!, dto);

            return Ok("Item added to cart");
        }

        // 🟢 GET CART
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = await _cartService.GetCartAsync(userId!);

            if (cart == null)
                return Ok("Cart is empty");

            return Ok(cart);
        }

        // 🟢 REMOVE ITEM
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _cartService.RemoveFromCartAsync(userId!, productId);

            return Ok("Item removed");
        }
    }
}