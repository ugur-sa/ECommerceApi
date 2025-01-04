using ECommerceApi.Application.Dtos;
using ECommerceApi.Application.Dtos.Category;
using ECommerceApi.Application.Dtos.Pagination;
using ECommerceApi.Application.Dtos.Product;
using ECommerceApi.Application.Helpers;
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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _dbContext;

        public CategoryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Category category)
        {
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await GetByIdAsync(id);
            if (category != null)
            {
                _dbContext.Categories.Remove(category);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _dbContext.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Categories
                .Include(c => c.Products)
                .Select(c => c)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<(IEnumerable<CategoryDto> Items, int TotalCount)> GetPaginatedAsync(QueryParameters parameters)
        {
            var query = _dbContext.Set<Category>().AsQueryable();

            query = query.ApplyFiltering(parameters);

            var totalCount = await query.CountAsync();

            var filteredSortedPagedQuery = query.ApplySorting(parameters).ApplyPagination(parameters);

            var items = await filteredSortedPagedQuery.Select(p => new CategoryDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            }).ToListAsync();

            return (items, totalCount);
        }

        public async Task UpdateAsync(Category category)
        {
            _dbContext.Entry(category).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
