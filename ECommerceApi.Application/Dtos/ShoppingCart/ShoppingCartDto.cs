using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Dtos.ShoppingCart
{
    public class ShoppingCartDto
    {
            public Guid UserId { get; set; }
            public List<ShoppingCartItemDto> Items { get; set; } = new List<ShoppingCartItemDto>();
            public decimal TotalPrice { get; set; }
    }
    public class ShoppingCartItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
