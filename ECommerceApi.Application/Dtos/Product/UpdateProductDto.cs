using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Application.Dtos.Product
{
    public class UpdateProductDto
    {
        [StringLength(100)]
        public string? Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal? Price { get; set; }

        public int? StockQuantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
    }
}
