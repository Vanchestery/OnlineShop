using OnlineShop.Core.Dtos;

namespace OnlineShop.Web.ViewModels.Favourites;

public class FavouriteItemViewModel
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal ProductPrice { get; init; }
    public string? ProductImagePath { get; init; }
    public bool ProductIsAvailable { get; init; }
    public DateTimeOffset AddedAt { get; init; }

    public static FavouriteItemViewModel FromDto(FavouriteItemDto dto) => new()
    {
        ProductId = dto.ProductId,
        ProductName = dto.ProductName,
        ProductPrice = dto.ProductPrice,
        ProductImagePath = dto.ProductImagePath,
        ProductIsAvailable = dto.ProductIsAvailable,
        AddedAt = dto.AddedAt
    };
}
