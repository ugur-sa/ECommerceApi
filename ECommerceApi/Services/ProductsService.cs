using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Services
{
    public class ProductsService
    {

        private readonly IProductRepository _productRepository;

        public ProductsService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();

            return products;
        }


        public async Task<Product?> GetProductByIdAsync(Guid Id)
        {
            var product = await _productRepository.GetByIdAsync(Id);

            return product;
        }

        public async Task AddProductAsync(Product product)
        {
            await _productRepository.AddAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(Guid Id)
        {
            await _productRepository.DeleteAsync(Id);
        }

    }
}
