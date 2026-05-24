using OnlineShop.Core.Dtos;

namespace OnlineShop.Web.Areas.Admin.ViewModels;

public class ProductRowViewModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string? ImagePath { get; init; }
    public bool IsAvailable { get; init; }

    public static ProductRowViewModel FromDto(ProductDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Price = dto.Price,
        ImagePath = dto.ImagePath,
        IsAvailable = dto.IsAvailable
    };
}
