using BLL.DTOs;
using BLL.Service.Abstraction;
using DAL.Database;
using DAL.Entities;
using DAL.Repo.Abstraction;

namespace BLL.Service.Impelementation
{
    public class CartService : ICartService
    {
        private readonly IGenericRepo<Cart> _cartRepo;
        private readonly IGenericRepo<CartItem> _itemRepo;
        private readonly IGenericRepo<Product> _productRepo;
        private readonly ApplicationDbContext _context;

        public CartService(
         IGenericRepo<Cart> cartRepo,
         IGenericRepo<CartItem> itemRepo,
         IGenericRepo<Product> productRepo,
         ApplicationDbContext context)
        {
            _cartRepo = cartRepo;
            _itemRepo = itemRepo;
            _productRepo = productRepo;
            _context = context;
        }

        // 🟢 ADD TO CART
        public async Task AddToCartAsync(string userId, CreateCartItemDto dto)
        {
            var product = await _productRepo.GetByIdAsync(dto.ProductId);

            if (product == null)
                return;

            var carts = await _cartRepo.FindAsync(c => c.UserId == userId);
            var cart = carts.FirstOrDefault();

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _cartRepo.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            var items = await _itemRepo.FindAsync(
                i => i.CartId == cart.Id && i.ProductId == dto.ProductId);

            var existingItem = items.FirstOrDefault();

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                _itemRepo.Update(existingItem);
            }
            else
            {
                await _itemRepo.AddAsync(new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                });
            }

            await _context.SaveChangesAsync();
        }

        // 🟢 GET CART
        public async Task<CartDto?> GetCartAsync(string userId)
        {
            var carts = await _cartRepo.FindAsync(c => c.UserId == userId);
            var cart = carts.FirstOrDefault();

            if (cart == null)
                return null;

            var items = await _itemRepo.FindAsync(i => i.CartId == cart.Id);

            var resultItems = new List<CartItemDto>();

            foreach (var item in items)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId);

                if (product == null)
                    continue;

                resultItems.Add(new CartItemDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = item.Quantity,
                    TotalPrice = product.Price * item.Quantity,
                    ImageUrl = product.ImageUrl
                });
            }

            return new CartDto
            {
                CartId = cart.Id,
                Items = resultItems
            };
        }

        // 🟢 REMOVE ITEM
        public async Task RemoveFromCartAsync(string userId, int productId)
        {
            var carts = await _cartRepo.FindAsync(c => c.UserId == userId);
            var cart = carts.FirstOrDefault();

            if (cart == null)
                return;

            var items = await _itemRepo.FindAsync(
                i => i.CartId == cart.Id && i.ProductId == productId);

            var item = items.FirstOrDefault();

            if (item == null)
                return;

            _itemRepo.Delete(item);

            await _context.SaveChangesAsync();
        }
        }
    }