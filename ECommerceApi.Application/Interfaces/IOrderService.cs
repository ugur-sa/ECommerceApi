using ECommerceApi.Application.Dtos.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Guid> CreateOrderAsync(CreateOrderDto orderDto);
        Task<OrderDto?> GetOrderByIdAsync(Guid id);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<IEnumerable<OrderDto>> GetOrderByUserId(Guid userId);
    }
}
