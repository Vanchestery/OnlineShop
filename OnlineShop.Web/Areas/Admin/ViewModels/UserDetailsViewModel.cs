using System.ComponentModel.DataAnnotations;
using OnlineShop.Core.Dtos;

namespace OnlineShop.Web.Areas.Admin.ViewModels;

public class UserDetailsViewModel
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Все роли в системе (для отображения чекбоксов).
    /// </summary>
    public IReadOnlyList<string> AllRoles { get; init; } = [];

    /// <summary>
    /// Роли которые сейчас у юзера.
    /// </summary>
    public IReadOnlyList<string> CurrentRoles { get; init; } = [];

    /// <summary>
    /// Вложенная VM для смены ролей (биндится из формы).
    /// </summary>
    public RolesAssignment RolesAssignment { get; init; } = new();

    /// <summary>
    /// Вложенная VM для сброса пароля.
    /// </summary>
    public PasswordReset PasswordReset { get; init; } = new();

    public static UserDetailsViewModel FromDto(UserDto dto, IReadOnlyList<string> allRoles) => new()
    {
        Id = dto.Id,
        Email = dto.Email,
        FullName = string.IsNullOrWhiteSpace(dto.FullName) ? "—" : dto.FullName,
        CreatedAt = dto.CreatedAt,
        AllRoles = allRoles,
        CurrentRoles = dto.Roles
    };
}

public class RolesAssignment
{
    public List<string> SelectedRoles { get; set; } = [];
}

public class PasswordReset
{
    [Required(ErrorMessage = "Введите новый пароль")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "От 6 до 100 символов")]
    [DataType(DataType.Password)]
    [Display(Name = "Новый пароль")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Подтвердите пароль")]
    [Compare(nameof(NewPassword), ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    [Display(Name = "Подтверждение")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
