namespace OnlineShop.Core.Dtos;

public record FavouriteItemDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal ProductPrice { get; init; }
    public string? ProductImagePath { get; init; }
    public bool ProductIsAvailable { get; init; }
    public DateTimeOffset AddedAt { get; init; }
}
