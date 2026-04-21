using Application.DTOs.MenuItemDTOs;
using Application.DTOs.OrderDTOs;
using Application.DTOs.OrderItemDTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping
{
    public class MappingContext : Profile
    {
        public MappingContext()
        {
            CreateMap<MenuItemDTO, MenuItem>().ReverseMap();

            CreateMap<OrderDTO, Order>().ReverseMap();
            CreateMap<Order, OrderListDTO>()
                .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.Items.Count));

            CreateMap<OrderItemDTO, OrderItem>().ReverseMap();
            CreateMap<OrderItem, OrderItemListDTO>();
        }
    }
}