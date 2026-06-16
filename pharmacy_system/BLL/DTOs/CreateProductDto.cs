using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = null!;
        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }
        public int BonusPoints { get; set; } = 0;
        public int FreeUnitsPerQuantity { get; set; } = 0;
        public int FreeUnitsCount { get; set; } = 0;

        public int StockQuantity { get; set; }

        public DateTime ExpiryDate { get; set; }

        [Required(ErrorMessage = "CategoryId is required")]
        public int CategoryId { get; set; }

        public string? ImageUrl { get; set; }
    }
}