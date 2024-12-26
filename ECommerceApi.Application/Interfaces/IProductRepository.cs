using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task AddAsync(Product product, Category category);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
    }
}
