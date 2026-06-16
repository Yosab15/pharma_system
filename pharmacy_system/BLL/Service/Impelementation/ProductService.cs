using BLL.DTOs;
using BLL.Service.Abstraction;
using DAL.Database;
using DAL.Entities;
using DAL.Repo.Abstraction;

namespace BLL.Service.Impelementation
{
    public class ProductService : IProductService
    {
        private readonly IProductRepo _productRepo;
        private readonly ApplicationDbContext _context;

        public ProductService(
            IProductRepo productRepo,
            ApplicationDbContext context)
        {
            _productRepo = productRepo;
            _context = context;
        }

        // GET ALL
        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepo.GetProductsWithCategoryAsync();
            return products
                .Where(p => !p.IsDeleted)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ExpiryDate = p.ExpiryDate,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category?.Name,
                    ImageUrl = p.ImageUrl,
                    BonusPoints = p.BonusPoints,
                    FreeUnitsPerQuantity = p.FreeUnitsPerQuantity,
                    FreeUnitsCount = p.FreeUnitsCount
                });
        }

        // GET BY ID
        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _productRepo.GetProductWithCategoryAsync(id);
            if (product == null)
                return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ExpiryDate = product.ExpiryDate,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                ImageUrl = product.ImageUrl,
                BonusPoints = product.BonusPoints,
                FreeUnitsPerQuantity = product.FreeUnitsPerQuantity,
                FreeUnitsCount = product.FreeUnitsCount
            };
        }

        // ADD
        public async Task AddAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                ExpiryDate = dto.ExpiryDate,
                CategoryId = dto.CategoryId,
                ImageUrl = dto.ImageUrl ?? "default.png",
                BonusPoints = dto.BonusPoints,
                FreeUnitsPerQuantity = dto.FreeUnitsPerQuantity,
                FreeUnitsCount = dto.FreeUnitsCount
            };
            await _productRepo.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        // UPDATE
        public async Task UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null)
                return;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.ExpiryDate = dto.ExpiryDate;
            product.CategoryId = dto.CategoryId;
            product.BonusPoints = dto.BonusPoints;
            product.FreeUnitsPerQuantity = dto.FreeUnitsPerQuantity;
            product.FreeUnitsCount = dto.FreeUnitsCount;

            if (!string.IsNullOrEmpty(dto.ImageUrl))
                product.ImageUrl = dto.ImageUrl;

            _productRepo.Update(product);
            await _context.SaveChangesAsync();
        }

        // DELETE - Soft Delete
        public async Task DeleteAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null)
                return;

            product.IsDeleted = true;
            _productRepo.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}