using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Dtos.ShoppingCart
{
    public class AddShoppingCartDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
