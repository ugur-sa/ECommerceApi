namespace ECommerceApi.Application.Dtos
{
    public class CategoryWithProductsDto
    {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public List<ProductDto>? Products { get; set; } 
    }
}
