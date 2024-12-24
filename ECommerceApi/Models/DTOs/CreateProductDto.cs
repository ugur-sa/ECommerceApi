using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Models.DTOs
{
    public class CreateProductDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 10000.0, ErrorMessage = "Price must be between 0.01 and 10,000.")]
        public decimal Price { get; set; }
    }
}
