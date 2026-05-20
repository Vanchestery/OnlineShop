using OnlineShop.Core.Dtos;

namespace OnlineShop.Core.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct = default);

    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
