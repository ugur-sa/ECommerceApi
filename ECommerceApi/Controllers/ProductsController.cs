using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Dtos;
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

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetProductById(Guid Id)
        {
            var product = await _productRepository.GetByIdAsync(Id);
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

        [HttpPatch("{Id}")]
        public async Task<IActionResult> UpdateProduct(Guid Id, [FromBody] UpdateProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _productRepository.GetByIdAsync(Id);
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
        public async Task<IActionResult> DeleteProduct(Guid Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _productRepository.DeleteAsync(Id);
            return NoContent();
        }
    }
}
