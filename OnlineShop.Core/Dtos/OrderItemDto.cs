namespace OnlineShop.Core.Dtos;

public record OrderItemDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    /// <summary>
    /// Снимок названия на момент покупки.
    /// </summary>
    public string ProductName { get; init; } = string.Empty;
    /// <summary>
    /// Снимок цены на момент покупки.
    /// </summary>
    public decimal Price { get; init; }
    public int Quantity { get; init; }
    public decimal Subtotal { get; init; }
}
