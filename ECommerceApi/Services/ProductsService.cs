using ECommerceApi.Data;
using ECommerceApi.Models;
using ECommerceApi.Models.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Services
{
    public class ProductsService
    {

        private readonly AppDbContext _dbContext;

        public ProductsService(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            var products = await _dbContext.Products.ToListAsync();

            return products;
        }


        public async Task<Product?> GetProductByIdAsync(Guid Id)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == Id);

            return product;
        }

        public async Task AddProductAsync(Product product)
        {
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateProductAsync(Guid Id, UpdateProductDto updateProductDto)
        {
            var product = await _dbContext.Products.FindAsync(Id);
            if (product == null) return false;

            if (IsMeaningful(updateProductDto.Name))
            {
                product.Name = updateProductDto.Name;
            }

            if (IsMeaningful(updateProductDto.Description))
            {
                product.Description = updateProductDto.Description;
            }

            if (IsMeaningful(updateProductDto.Price.ToString()))
            {
                product.Price = updateProductDto.Price;
            }

            product.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(Guid Id) 
        {
            var product = await _dbContext.Products.FindAsync(Id);
            if (product == null) return false;

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static bool IsMeaningful(string? value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

    }
}
