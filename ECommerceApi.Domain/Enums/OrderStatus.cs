using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 0,      // Initial status
        Processing = 1,   // Order is being processed
        Shipped = 2,      // Order has been shipped
        Delivered = 3,     // Order has been delivered
        Cancelled = 4
    }
}
