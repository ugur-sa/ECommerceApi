using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Domain.Entities
{
    public class ShoppingCartItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ShoppingCartId { get; set; } // Foreign key
        public Guid ProductId { get; set; } // Foreign key
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
