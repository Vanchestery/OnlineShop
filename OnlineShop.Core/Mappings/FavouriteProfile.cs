using AutoMapper;
using OnlineShop.Core.Dtos;
using OnlineShop.Db.Models;

namespace OnlineShop.Core.Mappings;

public class FavouriteProfile : Profile
{
    public FavouriteProfile()
    {
        CreateMap<FavouritesItem, FavouriteItemDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
            .ForMember(d => d.ProductPrice, opt => opt.MapFrom(s => s.Product != null ? s.Product.Price : 0m))
            .ForMember(d => d.ProductImagePath, opt => opt.MapFrom(s => s.Product != null ? s.Product.ImagePath : null))
            .ForMember(d => d.ProductIsAvailable, opt => opt.MapFrom(s => s.Product != null && s.Product.IsAvailable));
    }
}
