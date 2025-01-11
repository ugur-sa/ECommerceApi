using AutoMapper;
using ECommerceApi.Application.Dtos.Order;
using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty));
            CreateMap<OrderDto, Order>();
            CreateMap<CreateOrderDto, Order>();
        }

    }
}
