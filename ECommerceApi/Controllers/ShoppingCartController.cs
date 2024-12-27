using ECommerceApi.Application.Dtos.ShoppingCart;
using ECommerceApi.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetShoppingCart(Guid userId)
        {
            var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(userId);
            if (shoppingCart == null) return NotFound();

            return Ok(shoppingCart);
        }

        [HttpPost("{userId:guid}/items")]
        public async Task<IActionResult> AddItemToShoppingCart(Guid userId, [FromBody] AddShoppingCartDto itemDto)
        {
            try
            {
                await _shoppingCartService.AddItemToShoppingCartAsync(userId, itemDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{userId:guid}/items/{itemId:guid}")]
        public async Task<IActionResult> RemoveItemFromShoppingCart(Guid userId, Guid itemId)
        {
            await _shoppingCartService.RemoveItemFromShoppingCartAsync(userId, itemId);
            return NoContent();
        }

        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> ClearShoppingCart(Guid userId)
        {
            await _shoppingCartService.ClearShoppingCartAsync(userId);
            return NoContent();
        }
    }
}
