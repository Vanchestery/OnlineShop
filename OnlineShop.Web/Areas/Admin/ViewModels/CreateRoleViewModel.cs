using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Web.Areas.Admin.ViewModels;

public class CreateRoleViewModel
{
    [Required(ErrorMessage = "Имя роли обязательно")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "От 2 до 50 символов")]
    [RegularExpression(@"^[A-Za-z][A-Za-z0-9_]*$",
        ErrorMessage = "Только латинские буквы, цифры и подчёркивание, начало — с буквы")]
    [Display(Name = "Имя роли")]
    public string Name { get; set; } = string.Empty;
}
