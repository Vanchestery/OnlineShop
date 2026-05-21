using OnlineShop.Core.Dtos;

namespace OnlineShop.Web.ViewModels.Catalog;

/// <summary>
/// Расширенное представление товара для детальной страницы.
/// В Phase 8 добавятся поля Reviews и AverageRating.
/// </summary>
public class ProductDetailsViewModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string? ImagePath { get; init; }
    public bool IsAvailable { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public static ProductDetailsViewModel FromDto(ProductDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        Price = dto.Price,
        ImagePath = dto.ImagePath,
        IsAvailable = dto.IsAvailable,
        CreatedAt = dto.CreatedAt
    };
}
