using System.ComponentModel.DataAnnotations;
using DAL.Enums;

namespace BLL.DTOs
{
    public class CreateOrderDto
    {
        [Required]
        public string CustomerName { get; set; } = null!;
        [Required]
        public string ResponsibleName { get; set; } = null!;
        [Required]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        public string Address { get; set; } = null!;
        [Required]
        public EgyptianCity City { get; set; }
        public string? Notes { get; set; }
        public string? SupplyOrderImageUrl { get; set; } = null!;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string PaymentType { get; set; } = "Cash";
        public int FreeUnitsReceived { get; set; } = 0;
        public List<OrderItemDto> Items { get; set; }
            = new List<OrderItemDto>();
    }
}