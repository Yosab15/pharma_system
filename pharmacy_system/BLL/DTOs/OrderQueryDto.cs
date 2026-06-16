using DAL.Enums;

namespace BLL.DTOs
{
    public class OrderQueryDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public OrderStatus? Status { get; set; }
        public EgyptianCity? City { get; set; }
    }
}