using BLL.DTOs;
using BLL.Service.Abstraction;
using DAL.Enums;
using DAL.Specifications.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace pharmacy_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // CUSTOMER
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateOrderDto dto, IFormFile? supplyOrderImage)
        {
            // تأكد إن الصورة موجودة
            if (supplyOrderImage == null)
                return BadRequest(new { Success = false, Message = "Supply order image is required" });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(supplyOrderImage.FileName).ToLower();
            if (!allowedExtensions.Contains(ext))
                return BadRequest(new { Success = false, Message = "Only jpg and png images are allowed" });

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/orders");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var imageName = Guid.NewGuid().ToString() + ext;
            var filePath = Path.Combine(uploadsFolder, imageName);
            using (var stream = new FileStream(filePath, FileMode.Create))
                await supplyOrderImage.CopyToAsync(stream);

            dto.SupplyOrderImageUrl = "/orders/" + imageName;

            await _orderService.CreateOrderAsync(dto);
            return Ok(new { message = "Order Created" });
        }

        // ADMIN
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // ADMIN
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();
            return Ok(order);
        }

        // ADMIN
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusDto dto)
        {
            await _orderService.UpdateOrderStatusAsync(id, dto);
            return Ok("Status Updated");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(OrderStatus status)
        {
            var orders = await _orderService.GetOrdersByStatusAsync(status);
            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] OrderSpecParams query)
        {
            var result = await _orderService.GetOrdersAsync(query);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/discount")]
        public async Task<IActionResult> UpdateDiscount(int id, [FromBody] UpdateDiscountDto dto)
        {
            await _orderService.UpdateDiscountAsync(id, dto);
            return Ok("Discount Updated");
        }

    }
}