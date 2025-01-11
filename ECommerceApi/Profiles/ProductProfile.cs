using AutoMapper;
using ECommerceApi.Application.Dtos.Product;
using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
        }
    }
}
