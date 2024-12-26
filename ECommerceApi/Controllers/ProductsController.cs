using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;


        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if(product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        {

            var newProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _productRepository.AddAsync(newProduct);
            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
        }

        [HttpPatch("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (productDto.Name != null)
            {
                product.Name = productDto.Name;
            }

            if (productDto.Description != null)
            {
                product.Description = productDto.Description;
            }

            if (productDto.Price.HasValue)
            {
                product.Price = productDto.Price.Value;
            }

            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);

            return NoContent();
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _productRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
