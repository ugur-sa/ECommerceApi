using ECommerceApi.Domain.Entities;
using ECommerceApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsService _productsService;


        public ProductsController(ProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productsService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetProductById(Guid Id)
        {
            var product = await _productsService.GetProductByIdAsync(Id);
            if(product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {

            var newProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _productsService.AddProductAsync(newProduct);
            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            product.UpdatedAt = DateTime.Now;

            await _productsService.UpdateProductAsync(product);

            return NoContent();
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(Guid Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _productsService.DeleteProductAsync(Id);
            return NoContent();
        }
    }
}
