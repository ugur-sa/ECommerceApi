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
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly AppDbContext _dbContext;
        public ShoppingCartRepository(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task AddShoppingCartAsync(ShoppingCart shoppingCart)
        {
            await _dbContext.ShoppingCarts.AddAsync(shoppingCart);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteShoppingCartAsync(Guid userId)
        {
            var shoppingCart = await GetShoppingCartAsync(userId);
            if(shoppingCart != null)
            {
                _dbContext.ShoppingCarts.Remove(shoppingCart);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<ShoppingCart?> GetShoppingCartAsync(Guid userId)
        {
            return await _dbContext.ShoppingCarts.Include(b => b.Items).FirstOrDefaultAsync(b => b.UserId == userId);
        }

        public async Task UpdateShoppingCartAsync(ShoppingCart shoppingCart)
        {
            _dbContext.ShoppingCarts.Update(shoppingCart);
            await _dbContext.SaveChangesAsync();
        }
    }
}
