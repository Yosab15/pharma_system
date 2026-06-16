using DAL.Enums;

namespace DAL.Specifications.Orders
{
    public class OrderSpecParams
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }
        public EgyptianCity? City { get; set; }

        public OrderStatus? Status { get; set; }
    }
}