﻿using ECommerceApi.Application.Dtos.Order;
using ECommerceApi.Application.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            try
            {
                var orderId = await _orderService.CreateOrderAsync(orderDto);

                BackgroundJob.Schedule<IOrderService>(service => service.UpdateOrderStatus(orderId), TimeSpan.FromMinutes(2));

                return CreatedAtAction(nameof(GetOrderById), new { id = orderId }, null);
            }
            catch (InvalidOperationException ex)
            {
                // Return a 400 Bad Request with the exception message
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // For unexpected exceptions, return a 500 Internal Server Error
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            return Ok(order);
        }

        [HttpGet("users/{userId:guid}/orders")]
        public async Task<IActionResult> GetOrdersByUserId(Guid userId)
        {
            var userOrders = await _orderService.GetOrderByUserId(userId);
            if(userOrders == null) return NotFound();
            return Ok(userOrders);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }
    }
}
