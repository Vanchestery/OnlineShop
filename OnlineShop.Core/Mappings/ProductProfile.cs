using AutoMapper;
using OnlineShop.Core.Dtos;
using OnlineShop.Db.Models;

namespace OnlineShop.Core.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<ProductDto, Product>();
    }
}
