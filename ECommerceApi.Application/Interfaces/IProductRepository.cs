﻿using ECommerceApi.Application.Dtos.Pagination;
using ECommerceApi.Application.Dtos.Product;
using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<(IEnumerable<ProductDto> Items, int TotalCount)> GetAllAsync(QueryParameters queryParameters);
        Task<Product?> GetByIdAsync(Guid id);
        Task AddAsync(Product product, Category category);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
    }
}
