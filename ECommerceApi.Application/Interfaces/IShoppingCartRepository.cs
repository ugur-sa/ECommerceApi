using ECommerceApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Interfaces
{
    public interface IShoppingCartRepository
    {
        Task<ShoppingCart?> GetShoppingCartAsync(Guid userId);
        Task AddShoppingCartAsync(ShoppingCart shoppingCart);
        Task UpdateShoppingCartAsync(ShoppingCart shoppingCart);
        Task DeleteShoppingCartAsync(Guid userId);
        Task DeleteAllShoppingCartItemsAsync(Guid userId);
    }
}
