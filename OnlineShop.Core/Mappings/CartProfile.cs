using AutoMapper;
using OnlineShop.Core.Dtos;
using OnlineShop.Db.Models;

namespace OnlineShop.Core.Mappings;

public class CartProfile : Profile
{
    public CartProfile()
    {
        // CartItem с подтянутой навигацией Product — берём оттуда название/цену/картинку.
        CreateMap<CartItem, CartItemDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
            .ForMember(d => d.ProductPrice, opt => opt.MapFrom(s => s.Product != null ? s.Product.Price : 0m))
            .ForMember(d => d.ProductImagePath, opt => opt.MapFrom(s => s.Product != null ? s.Product.ImagePath : null))
            .ForMember(d => d.Subtotal, opt => opt.MapFrom(s => s.Product != null ? s.Product.Price * s.Quantity : 0m));

        CreateMap<Cart, CartDto>()
            .ForMember(d => d.Total, opt => opt.MapFrom(s =>
                s.Items.Sum(i => (i.Product != null ? i.Product.Price : 0m) * i.Quantity)));
    }
}
