using BLL.DTOs;
using BLL.Service.Abstraction;
using DAL.Database;
using DAL.Entities;
using DAL.Enums;
using DAL.Repo.Abstraction;
using DAL.Specifications.Orders;
using DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace BLL.Service.Impelementation
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public OrderService(
            IUnitOfWork unitOfWork,
            ApplicationDbContext context,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _emailService = emailService;
        }

        public async Task CreateOrderAsync(CreateOrderDto dto)
        {
            var order = new Order
            {
                CustomerName = dto.CustomerName,
                ResponsibleName = dto.ResponsibleName,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                City = dto.City,
                Notes = dto.Notes,
                SupplyOrderImageUrl = dto.SupplyOrderImageUrl,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                PaymentType = dto.PaymentType,
                FreeUnitsReceived = dto.FreeUnitsReceived,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };

            decimal total = 0;

            foreach (var item in dto.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product == null)
                    continue;

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Price = product.Price
                });

                total += product.Price * item.Quantity;
            }

            order.TotalPrice = total;

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }
        private async Task SendEmailAsync(Order order)
        {
            // SMTP / MailKit
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerName = o.CustomerName,
                    PhoneNumber = o.PhoneNumber,
                    Address = o.Address,
                    Notes = o.Notes,
                    SupplyOrderImageUrl = o.SupplyOrderImageUrl, // ✅
                    TotalPrice = o.TotalPrice,
                    OrderDate = o.OrderDate,
                    Status = o.Status,

                    Items = o.OrderItems.Select(i => new OrderItemDetailsDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.Product.Name,
                        Price = i.Price,
                        Quantity = i.Quantity,
                        Total = i.Price * i.Quantity
                    }).ToList()
                })
                .ToListAsync();

            return orders;
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Where(o => o.Id == id)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerName = o.CustomerName,
                    ResponsibleName = o.ResponsibleName,
                    PhoneNumber = o.PhoneNumber,
                    Address = o.Address,
                    City = o.City,
                    Notes = o.Notes,
                    SupplyOrderImageUrl = o.SupplyOrderImageUrl,
                    Latitude = o.Latitude,
                    Longitude = o.Longitude,
                    PaymentType = o.PaymentType,
                    FreeUnitsReceived = o.FreeUnitsReceived,
                    TotalPrice = o.TotalPrice,
                    OrderDate = o.OrderDate,
                    Status = o.Status,

                    Items = o.OrderItems.Select(i => new OrderItemDetailsDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.Product != null ? i.Product.Name : "Deleted Product",
                        Price = i.Price,
                        Quantity = i.Quantity,
                        Total = i.Price * i.Quantity
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }


        public async Task UpdateOrderStatusAsync(int id, UpdateOrderStatusDto dto)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return;

            if (order.Status == dto.Status)
                return;

            order.Status = dto.Status;
            order.LastStatusUpdate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerName = o.CustomerName,
                    PhoneNumber = o.PhoneNumber,
                    Address = o.Address,
                    Notes = o.Notes,
                    SupplyOrderImageUrl = o.SupplyOrderImageUrl, // ✅
                    TotalPrice = o.TotalPrice,
                    OrderDate = o.OrderDate,
                    Status = o.Status,

                    Items = o.OrderItems.Select(i => new OrderItemDetailsDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.Product.Name,
                        Price = i.Price,
                        Quantity = i.Quantity,
                        Total = i.Price * i.Quantity
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<PaginatedResultDto<OrderDto>> GetOrdersAsync(OrderSpecParams query)
        {
            var spec = new OrderSpecification(query);

            var orders = await _unitOfWork
                .Orders
                .GetAllWithSpecAsync(spec);

            var countSpec = new OrderCountSpecification(query);
            var totalCount = await _unitOfWork
                .Orders
                .CountAsync(countSpec);

            var data = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                PhoneNumber = o.PhoneNumber,
                Address = o.Address,
                City = o.City,
                Notes = o.Notes,
                SupplyOrderImageUrl = o.SupplyOrderImageUrl, // ✅
                TotalPrice = o.TotalPrice,
                OrderDate = o.OrderDate,
                Status = o.Status
            }).ToList();

            return new PaginatedResultDto<OrderDto>
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                Data = data
            };
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var today = DateTime.UtcNow.Date;

            var stats = new DashboardStatsDto
            {
                TotalOrders = await _context.Orders.CountAsync(),

                PendingOrders = await _context.Orders
                    .CountAsync(o => o.Status == OrderStatus.Pending),

                DeliveredOrders = await _context.Orders
                    .CountAsync(o => o.Status == OrderStatus.Delivered),

                TotalRevenue = await _context.Orders
                    .Where(o => o.Status == OrderStatus.Delivered)
                    .SumAsync(o => (decimal?)o.TotalPrice) ?? 0,

                TodayOrders = await _context.Orders
                    .CountAsync(o => o.OrderDate.Date == today),

                TodayRevenue = await _context.Orders
                    .Where(o =>
                        o.OrderDate.Date == today &&
                        o.Status == OrderStatus.Delivered)
                    .SumAsync(o => (decimal?)o.TotalPrice) ?? 0
            };

            return stats;
        }

        public async Task<IEnumerable<TopProductDto>> GetTopProductsAsync()
        {
            return await _context.OrderItems
                .GroupBy(i => new
                {
                    i.ProductId,
                    i.Product.Name
                })
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    TotalQuantity = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(5)
                .ToListAsync();
        }


        public async Task UpdateDiscountAsync(int id, UpdateDiscountDto dto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return;

            order.Discount = dto.Discount;
            order.FinalPrice = order.TotalPrice - dto.Discount;
            order.LastStatusUpdate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

    }

}