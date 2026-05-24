using OnlineShop.Core.Dtos;

namespace OnlineShop.Web.Areas.Admin.ViewModels;

public class UserRowViewModel
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = [];

    public static UserRowViewModel FromDto(UserDto dto) => new()
    {
        Id = dto.Id,
        Email = dto.Email,
        FullName = string.IsNullOrWhiteSpace(dto.FullName) ? "—" : dto.FullName,
        CreatedAt = dto.CreatedAt,
        Roles = dto.Roles
    };
}
