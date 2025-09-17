using AutoMapper;
using GestionPedidosAV.Application.DTOs;
using GestionPedidosAV.Domain.Entities;

namespace GestionPedidosAV.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();
        
        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsAvailable));
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
        
        // Order mappings
        CreateMap<Order, OrderDto>();
        CreateMap<CreateOrderDto, Order>();
        
        // OrderItem mappings
        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<CreateOrderItemDto, OrderItem>();
    }
}