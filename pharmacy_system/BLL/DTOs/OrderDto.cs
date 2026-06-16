using DAL.Enums;

namespace BLL.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = null!;
        public string ResponsibleName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public EgyptianCity City { get; set; }
        public string? Notes { get; set; }
        public string? SupplyOrderImageUrl { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public decimal? Discount { get; set; }
        public decimal? FinalPrice { get; set; }
        public string PaymentType { get; set; } = "Cash";
        public int FreeUnitsReceived { get; set; } = 0;

        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItemDetailsDto> Items { get; set; }
            = new();
    }
}