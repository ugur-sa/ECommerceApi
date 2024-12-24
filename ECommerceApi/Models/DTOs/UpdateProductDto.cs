﻿using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Models.DTOs
{
    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }
}
