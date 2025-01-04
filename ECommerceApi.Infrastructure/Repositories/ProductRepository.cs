using ECommerceApi.Application.Dtos;
using ECommerceApi.Application.Dtos.Pagination;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Application.Dtos.Product;
using ECommerceApi.Application.Helpers;

namespace ECommerceApi.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(IEnumerable<ProductDto> Items, int TotalCount)> GetAllAsync(QueryParameters parameters)
        {
            var query = _dbContext.Set<Product>().Include(p => p.Category).AsQueryable();

            // Apply filtering (filtering logic is in ApplyQueryParameters)
            query = query.ApplyFiltering(parameters);

            // Calculate the total count AFTER filtering but BEFORE pagination
            var totalCount = await query.CountAsync();

            // Apply sorting and pagination
            var filteredSortedPagedQuery = query.ApplySorting(parameters).ApplyPagination(parameters);

            var items = await filteredSortedPagedQuery.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category.Name,
                CategoryId = p.CategoryId
            }).ToListAsync();

            return (items, totalCount);
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Select(p => p)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product, Category category)
        {
            _dbContext.Entry(category).State = EntityState.Unchanged;


            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await GetByIdAsync(id);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
