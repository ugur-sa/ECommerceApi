using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Dtos.Order
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; } = String.Empty;
        public decimal TotalPrice { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }

    public class OrderItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = String.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
