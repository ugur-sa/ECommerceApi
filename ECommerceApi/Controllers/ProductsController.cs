﻿using AutoMapper;
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
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] QueryParameters parameters)
        {
            var (items, totalCount) = await _productRepository.GetAllAsync(parameters);

            return Ok(new
            {
                TotalItems = totalCount,
                parameters.PageNumber,
                parameters.PageSize,
                Items = items
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            var productDtoResult = _mapper.Map<ProductDto>(product);

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

            var product = _mapper.Map<Product>(productDto);

            await _productRepository.AddAsync(product, category);

            var productDtoResult = _mapper.Map<ProductDto>(product);

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

            if (productDto.StockQuantity.HasValue)
            {
                product.StockQuantity = productDto.StockQuantity.Value;
            }

            if (productDto.CategoryId is not null)
            {
                product.CategoryId = (Guid)productDto.CategoryId;
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
