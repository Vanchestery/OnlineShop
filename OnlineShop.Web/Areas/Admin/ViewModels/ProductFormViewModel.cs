using System.ComponentModel.DataAnnotations;
using OnlineShop.Core.Dtos;

namespace OnlineShop.Web.Areas.Admin.ViewModels;

/// <summary>
/// Универсальная форма для Create/Edit товара. На Create Id=null, на Edit заполнен.
/// </summary>
public class ProductFormViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Название обязательно")]
    [StringLength(200, ErrorMessage = "Не более 200 символов")]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Описание обязательно")]
    [StringLength(4000, ErrorMessage = "Не более 4000 символов")]
    [Display(Name = "Описание")]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 99999999.99, ErrorMessage = "Цена должна быть положительной")]
    [Display(Name = "Цена (₽)")]
    public decimal Price { get; set; }

    [Display(Name = "Доступен к покупке")]
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// Существующая картинка (для отображения на Edit). null если ещё нет.
    /// </summary>
    public string? ExistingImagePath { get; set; }

    /// <summary>
    /// Новый загружаемый файл картинки. Опциональный — на Edit можно не менять.
    /// На Create — если пусто, у товара просто не будет картинки.
    /// </summary>
    [Display(Name = "Картинка (опционально)")]
    public IFormFile? ImageFile { get; set; }

    public static ProductFormViewModel FromDto(ProductDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        Price = dto.Price,
        IsAvailable = dto.IsAvailable,
        ExistingImagePath = dto.ImagePath
    };
}
