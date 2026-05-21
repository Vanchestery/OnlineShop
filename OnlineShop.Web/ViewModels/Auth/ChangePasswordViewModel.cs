using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Web.ViewModels.Auth;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Введите текущий пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Текущий пароль")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите новый пароль")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "От 6 до 100 символов")]
    [DataType(DataType.Password)]
    [Display(Name = "Новый пароль")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Подтвердите новый пароль")]
    [Compare(nameof(NewPassword), ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    [Display(Name = "Подтверждение нового пароля")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
