using ECommerceApi.Application.Dtos.Category;
using ECommerceApi.Application.Dtos.Pagination;
using ECommerceApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<(IEnumerable<CategoryDto> Items, int TotalCount)> GetPaginatedAsync(QueryParameters queryParameters);
        Task<Category?> GetByIdAsync(Guid id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Guid id);
    }
}
