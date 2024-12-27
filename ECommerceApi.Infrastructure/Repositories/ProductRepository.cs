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

namespace ECommerceApi.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginatedResponse<Product>> GetAllAsync(int pageNumber, int pageSize)
        {
            //return await _dbContext.Products
            //    .Include(p => p.Category)
            //    .Select(p => p)
            //    .ToListAsync();
            var totalItems = await _dbContext.Products.CountAsync();

            var items = await _dbContext.Products
                .Include(p => p.Category) // Ensure the Category is loaded for mapping
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Product>(items, totalItems, pageNumber, pageSize);
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
