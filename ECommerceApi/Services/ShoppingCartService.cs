﻿using ECommerceApi.Application.Dtos.ShoppingCart;
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
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                }).ToList(),
                TotalPrice = shoppingCart.TotalPrice,
            };
        }

        public async Task AddItemToShoppingCartAsync(Guid userId, AddShoppingCartDto itemDto)
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
            }

            // Pass the updated shopping cart to the repository
            await _shoppingCartRepository.UpdateShoppingCartAsync(shoppingCart);
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
    }
}