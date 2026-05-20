using AutoMapper;
using OnlineShop.Core.Dtos;
using OnlineShop.Core.Dtos.Requests;
using OnlineShop.Db.Models;

namespace OnlineShop.Core.Mappings;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<Review, ReviewDto>()
            .ForMember(d => d.UserName, opt => opt.MapFrom(s =>
                s.User != null ? (s.User.FirstName + " " + s.User.LastName).Trim() : "Аноним"));

        CreateMap<AddReviewRequest, Review>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.ProductId, opt => opt.Ignore())
            .ForMember(d => d.Product, opt => opt.Ignore())
            .ForMember(d => d.UserId, opt => opt.Ignore())
            .ForMember(d => d.User, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore());
    }
}
