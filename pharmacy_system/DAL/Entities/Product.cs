using DAL.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int BonusPoints { get; set; } = 0;
    public int FreeUnitsPerQuantity { get; set; } = 0; // كل كام وحدة
    public int FreeUnitsCount { get; set; } = 0;        // كام وحدة مجاناً
    public int StockQuantity { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string ImageUrl { get; set; } = null!;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; }
        = new HashSet<OrderItem>();
    public bool IsDeleted { get; set; } = false;
}