using ECommerceApi.Application.Dtos.Product;

namespace ECommerceApi.Application.Dtos.Category
{
    public class CategoryWithProductsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<ProductDto>? Products { get; set; }
    }
}
