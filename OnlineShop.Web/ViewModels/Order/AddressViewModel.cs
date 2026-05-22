using System.ComponentModel.DataAnnotations;
using OnlineShop.Core.Dtos;

namespace OnlineShop.Web.ViewModels.Order;

public class AddressViewModel
{
    [Required(ErrorMessage = "Страна обязательна")]
    [StringLength(100, ErrorMessage = "Не более 100 символов")]
    [Display(Name = "Страна")]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "Город обязателен")]
    [StringLength(100, ErrorMessage = "Не более 100 символов")]
    [Display(Name = "Город")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Улица и дом обязательны")]
    [StringLength(200, ErrorMessage = "Не более 200 символов")]
    [Display(Name = "Улица, дом")]
    public string Street { get; set; } = string.Empty;

    [Required(ErrorMessage = "Индекс обязателен")]
    [StringLength(20, ErrorMessage = "Не более 20 символов")]
    [Display(Name = "Почтовый индекс")]
    public string ZipCode { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Не более 50 символов")]
    [Display(Name = "Квартира (опционально)")]
    public string? Apartment { get; set; }

    public AddressDto ToDto() => new()
    {
        Country = Country.Trim(),
        City = City.Trim(),
        Street = Street.Trim(),
        ZipCode = ZipCode.Trim(),
        Apartment = string.IsNullOrWhiteSpace(Apartment) ? null : Apartment.Trim()
    };
}
