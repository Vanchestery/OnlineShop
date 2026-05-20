using AutoMapper;
using Microsoft.AspNetCore.Identity;
using OnlineShop.Core.Dtos;
using OnlineShop.Core.Interfaces;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;

namespace OnlineShop.Core.Services;

public class UserService : IUserService
{
    private readonly IUsersStorage _users;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public UserService(
        IUsersStorage users,
        UserManager<User> userManager,
        IMapper mapper)
    {
        _users = users;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct = default)
    {
        var users = await _users.GetAllAsync(ct);
        var result = new List<UserDto>(users.Count);
        foreach (var user in users)
        {
            var dto = _mapper.Map<UserDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(dto with { Roles = roles.ToList() });
        }
        return result;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(id, ct);
        if (user is null) return null;

        var dto = _mapper.Map<UserDto>(user);
        var roles = await _userManager.GetRolesAsync(user);
        return dto with { Roles = roles.ToList() };
    }
}
