using OnlineShop.Core.Dtos;
using OnlineShop.Db.Models;

namespace OnlineShop.Web.ViewModels.Catalog;

/// <summary>
/// Компактное представление товара для карточки в каталоге.
/// </summary>
public class ProductCardViewModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public ProductCategory Category { get; init; }
    public string? ImagePath { get; init; }
    public bool IsAvailable { get; init; }

    public static ProductCardViewModel FromDto(ProductDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Price = dto.Price,
        Category = dto.Category,
        ImagePath = dto.ImagePath,
        IsAvailable = dto.IsAvailable
    };
}
