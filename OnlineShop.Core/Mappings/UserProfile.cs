using AutoMapper;
using OnlineShop.Core.Dtos;
using OnlineShop.Db.Models;

namespace OnlineShop.Core.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email ?? string.Empty))
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => (s.FirstName + " " + s.LastName).Trim()))
            // Roles заполняем отдельно в сервисе через UserManager.GetRolesAsync.
            .ForMember(d => d.Roles, opt => opt.Ignore());
    }
}
