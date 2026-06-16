using BLL.DTOs;
using BLL.Service.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace pharmacy_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET ALL
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        // GET BY ID
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound("Product not found");
            return Ok(product);
        }

        // CREATE
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] CreateProductDto dto, IFormFile? image)
        {
            if (image != null)
            {
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (!_allowedExtensions.Contains(ext))
                    return BadRequest("Only jpg and png images are allowed");

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var imageName = Guid.NewGuid().ToString() + ext;
                var filePath = Path.Combine(uploadsFolder, imageName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await image.CopyToAsync(stream);

                dto.ImageUrl = "/images/" + imageName;
            }

            await _productService.AddAsync(dto);
            return Ok("Created Successfully");
        }

        // UPDATE
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductDto dto, IFormFile? image)
        {
            if (image != null)
            {
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (!_allowedExtensions.Contains(ext))
                    return BadRequest("Only jpg and png images are allowed");

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var imageName = Guid.NewGuid().ToString() + ext;
                var filePath = Path.Combine(uploadsFolder, imageName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await image.CopyToAsync(stream);

                dto.ImageUrl = "/images/" + imageName;
            }

            await _productService.UpdateAsync(id, dto);
            return Ok("Updated Successfully");
        }

        // DELETE
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return Ok("Product deleted successfully");
        }
    }
}