using ECommerceApi.Application.Dtos.ShoppingCart;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Application.Interfaces.Services;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IProductRepository _productRepository;
        private readonly AppDbContext _dbContext;

        public ShoppingCartService(IShoppingCartRepository shoppingCartRepository, IProductRepository productRepository, AppDbContext context)
        {
            _productRepository = productRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _dbContext = context;
        }
        public async Task<ShoppingCartDto?> GetShoppingCartAsync(Guid userId)
        {
            var shoppingCart = await _shoppingCartRepository.GetShoppingCartAsync(userId);
            if (shoppingCart == null) return null;

            return new ShoppingCartDto
            {
                UserId = userId,
                Items = shoppingCart.Items.Select(i => new ShoppingCartItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                }).ToList(),
                TotalPrice = shoppingCart.TotalPrice,
            };
        }

        public async Task<Guid> AddItemToShoppingCartAsync(Guid userId, AddShoppingCartDto itemDto)
        {
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
            if (product == null) throw new Exception("Product not found");

            var shoppingCart = await _shoppingCartRepository.GetShoppingCartAsync(userId);
            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart { UserId = userId };
                await _shoppingCartRepository.AddShoppingCartAsync(shoppingCart);
            }
            var existingItem = shoppingCart.Items.FirstOrDefault(i => i.ProductId == itemDto.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += itemDto.Quantity;
                await _shoppingCartRepository.UpdateShoppingCartAsync(shoppingCart);

                return existingItem.Id;
            }
            else
            {
                var newItem = new ShoppingCartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ShoppingCartId = shoppingCart.Id,
                    UnitPrice = product.Price,
                    Quantity = itemDto.Quantity
                };
                shoppingCart.Items.Add(newItem);
                _dbContext.Entry(newItem).State = EntityState.Added;

                await _shoppingCartRepository.UpdateShoppingCartAsync(shoppingCart);

                return newItem.Id;
            }
        }

        public async Task ClearShoppingCartAsync(Guid userId)
        {
            await _shoppingCartRepository.DeleteShoppingCartAsync(userId);
        }


        public async Task RemoveItemFromShoppingCartAsync(Guid userId, Guid itemId)
        {
            var shoppingCart = await _shoppingCartRepository.GetShoppingCartAsync(userId);
            if (shoppingCart == null) return;

            var item = shoppingCart.Items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                shoppingCart.Items.Remove(item);
                await _shoppingCartRepository.UpdateShoppingCartAsync(shoppingCart);
            }
        }

        public async Task UpdateShoppingCartItemQuantityAsync(Guid userId, Guid itemId, int quantity)
        {
            var shoppingCart = await _shoppingCartRepository.GetShoppingCartAsync(userId);
            if (shoppingCart == null) throw new Exception("Shopping cart not found");

            var cartItem = shoppingCart.Items.FirstOrDefault(item => item.Id == itemId);
            if (cartItem == null) throw new Exception("Item not found in shopping cart");

            if (quantity <= 0)
            {
                // Remove the item if the quantity is zero or less
                shoppingCart.Items.Remove(cartItem);
            }
            else
            {
                var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                if (product is null) return;
                if(product.StockQuantity < quantity)
                {
                    throw new Exception("Not enough stock");
                }

                // Update the item's quantity
                cartItem.Quantity = quantity;
            }

            await _shoppingCartRepository.UpdateShoppingCartAsync(shoppingCart);
        }
    }
}
