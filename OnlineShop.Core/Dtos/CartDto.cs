namespace OnlineShop.Core.Dtos;

public record CartDto
{
    public Guid Id { get; init; }
    /// <summary>
    /// null — анонимная корзина (Cart.Id хранится в куке).
    /// </summary>
    public Guid? UserId { get; init; }
    public IReadOnlyList<CartItemDto> Items { get; init; } = [];
    /// <summary>
    /// Сумма по всем позициям.
    /// </summary>
    public decimal Total { get; init; }
}
