using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceApi.Application.Dtos.Product;
using ECommerceApi.Application.Dtos.Pagination;

namespace ECommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] PaginationParameters paginationParameters)
        {
            var paginatedProducts = await _productRepository.GetAllAsync(paginationParameters.PageNumber, paginationParameters.PageSize);

            var productDtos = paginatedProducts.Data.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name
            });

            var response = new PaginatedResponse<ProductDto>(
                productDtos,
                paginatedProducts.TotalCount,
                paginatedProducts.CurrentPage,
                paginatedProducts.PageSize
            );

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            var productDtoResult = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name
            };

            return Ok(productDtoResult);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        {
            // Check if the category exists
            var category = await _categoryRepository.GetByIdAsync(productDto.CategoryId);
            if (category == null)
                return BadRequest("Invalid category.");
            
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                CategoryId = category.Id,
                ImageUrl = productDto.ImageUrl,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _productRepository.AddAsync(product, category);

            var productDtoResult = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name
            };

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, productDtoResult);
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

            if (productDto.ImageUrl != null)
            {
                product.ImageUrl = productDto.ImageUrl;
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
        [Authorize(Roles = "Admin")]
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
