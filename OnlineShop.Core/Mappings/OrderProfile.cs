using AutoMapper;
using OnlineShop.Core.Dtos;
using OnlineShop.Db.Models;

namespace OnlineShop.Core.Mappings;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Address, AddressDto>();
        CreateMap<AddressDto, Address>();

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.Subtotal, opt => opt.MapFrom(s => s.Price * s.Quantity));

        CreateMap<Order, OrderDto>()
            .ForMember(d => d.UserFullName, opt => opt.MapFrom(s =>
                s.User != null ? (s.User.FirstName + " " + s.User.LastName).Trim() : string.Empty));
    }
}
