namespace OnlineShop.Core.Dtos;

public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    /// <summary>
    /// Список ролей юзера. Заполняется сервисом отдельно (через UserManager.GetRolesAsync).
    /// </summary>
    public IReadOnlyList<string> Roles { get; init; } = [];
}
