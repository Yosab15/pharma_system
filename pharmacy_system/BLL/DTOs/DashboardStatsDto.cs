namespace BLL.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalOrders { get; set; }

        public int PendingOrders { get; set; }

        public int DeliveredOrders { get; set; }

        public decimal TotalRevenue { get; set; }

        public int TodayOrders { get; set; }

        public decimal TodayRevenue { get; set; }
    }
}