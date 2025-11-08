using AutoMapper;
using Prueba.Domain.Common;
using Prueba.Domain.Entities;
using Prueba.Application.Dtos;
using System.Linq;

namespace Prueba.Application.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>();

            
            CreateMap(typeof(PaginatedResult<>), typeof(PaginatedResultDto<>));

            
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Roles,
                           opt => opt.MapFrom(src => src.Roles.Select(role => role.Name).ToList()));

            CreateMap<Order, OrderDto>();
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName,
                           opt => opt.MapFrom(src => src.Product.Name));
        }
    }
}