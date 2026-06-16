namespace BLL.DTOs
{
    public class TopProductDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public int TotalQuantity { get; set; }
    }
}