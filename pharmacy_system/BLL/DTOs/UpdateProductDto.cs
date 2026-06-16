namespace BLL.DTOs
{
    public class UpdateProductDto
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public int BonusPoints { get; set; } = 0;
        public int FreeUnitsPerQuantity { get; set; } = 0;
        public int FreeUnitsCount { get; set; } = 0;

        public int StockQuantity { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int CategoryId { get; set; }

        public string? ImageUrl { get; set; }
    }
}