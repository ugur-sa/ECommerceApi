using ECommerceApi.Application.Dtos.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Interfaces.Services
{
    public interface IShoppingCartService
    {
        Task<ShoppingCartDto?> GetShoppingCartAsync(Guid userId);
        Task<Guid> AddItemToShoppingCartAsync(Guid userId, AddShoppingCartDto itemDto);
        Task RemoveItemFromShoppingCartAsync(Guid userId, Guid itemId);
        Task ClearShoppingCartAsync(Guid userId);
        Task UpdateShoppingCartItemQuantityAsync(Guid guid, Guid itemId, int quantity);
    }
}
