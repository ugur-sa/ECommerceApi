using ECommerceApi.Application.Dtos.Order;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Enums;
using Hangfire;

namespace ECommerceApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IShoppingCartRepository shoppingCartRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _shoppingCartRepository = shoppingCartRepository;
        }

        public async Task<Guid> CreateOrderAsync(CreateOrderDto orderDto)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = orderDto.CustomerId,
                OrderDate = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OrderNumber = GenerateOrderNumber(),
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in orderDto.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new Exception($"Product with ID {item.ProductId} not found.");
                }

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });
            }

            await _orderRepository.AddAsync(order);
            await _shoppingCartRepository.DeleteAllShoppingCartItemsAsync(order.CustomerId);
            
            return order.Id;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                OrderDate = o.OrderDate,
                UpdatedAt = o.UpdatedAt,
                OrderNumber = o.OrderNumber,
                Status = o.Status,
                TotalPrice = o.TotalPrice,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? string.Empty,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            });
        }

        public async Task<IEnumerable<OrderDto>> GetOrderByUserId(Guid userId)
        {
            var orders = await _orderRepository.GetAllUserOrdersAsync(userId);
            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                OrderDate = o.OrderDate,
                UpdatedAt = o.UpdatedAt,
                OrderNumber = o.OrderNumber,
                Status = o.Status,
                TotalPrice = o.TotalPrice,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList(),
            });
        }

        public async Task<OrderDto?> GetOrderByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return null;

            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                UpdatedAt = order.UpdatedAt,
                OrderNumber = order.OrderNumber,
                Status = order.Status,
                TotalPrice = order.TotalPrice,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? string.Empty,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
        }

        public async Task UpdateOrderStatus(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order is null) throw new InvalidOperationException("Order not found.");

            var nextStatus = GetNextStatus(order.Status);

            if(nextStatus != order.Status)
            {
                order.Status = GetNextStatus(order.Status);
                order.UpdatedAt = DateTime.UtcNow;

                await _orderRepository.UpdateAsync(order);

                BackgroundJob.Schedule<IOrderService>(
                    service => service.UpdateOrderStatus(orderId),
                    TimeSpan.FromMinutes(2));
            }
        }

        private OrderStatus GetNextStatus(OrderStatus currentStatus)
        {
            return currentStatus switch
            {
                OrderStatus.Pending => OrderStatus.Processing,
                OrderStatus.Processing => OrderStatus.Shipped,
                OrderStatus.Shipped => OrderStatus.Delivered,
                _ => currentStatus
            };
        }
        private string GenerateOrderNumber()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return "ORD-" + new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }
    }
}
