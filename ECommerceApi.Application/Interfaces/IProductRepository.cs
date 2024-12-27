using ECommerceApi.Application.Dtos.Pagination;
using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<PaginatedResponse<Product>> GetAllAsync(int pageNumber, int pageSize);
        Task<Product?> GetByIdAsync(Guid id);
        Task AddAsync(Product product, Category category);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
    }
}
