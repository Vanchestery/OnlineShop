namespace OnlineShop.Core.Dtos;

public record CartItemDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal ProductPrice { get; init; }
    public string? ProductImagePath { get; init; }
    public int Quantity { get; init; }
    public DateTimeOffset AddedAt { get; init; }

    /// <summary>
    /// Подсумма по позиции = ProductPrice * Quantity.
    /// </summary>
    public decimal Subtotal { get; init; }
}
