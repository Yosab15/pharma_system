using BLL.DTOs;
using DAL.Enums;
using DAL.Specifications.Orders;

namespace BLL.Service.Abstraction
{
    public interface IOrderService
    {
        // Customer
        Task CreateOrderAsync(CreateOrderDto dto);

        // Admin
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();

        Task<OrderDto?> GetOrderByIdAsync(int id);

        Task UpdateOrderStatusAsync(int id,UpdateOrderStatusDto dto);
        Task<PaginatedResultDto<OrderDto>> GetOrdersAsync(OrderSpecParams query);

        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status);

        Task<DashboardStatsDto> GetDashboardStatsAsync();

        Task<IEnumerable<TopProductDto>> GetTopProductsAsync();

        Task UpdateDiscountAsync(int id, UpdateDiscountDto dto);

    }
}