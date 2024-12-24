﻿using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Dtos
{
    public class UpdateProductDto
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; } = String.Empty;

        [StringLength(500)]
        public string? Description { get; set; } = String.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal? Price { get; set; }
    }
}
