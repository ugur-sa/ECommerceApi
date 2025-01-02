using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; } = String.Empty; // e.g., "Pending", "Completed", "Cancelled"
        public decimal TotalPrice => OrderItems.Sum(item => item.UnitPrice * item.Quantity);

        // Navigation property
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
