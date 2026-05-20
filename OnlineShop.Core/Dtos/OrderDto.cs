using OnlineShop.Db.Models;

namespace OnlineShop.Core.Dtos;

public record OrderDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string UserFullName { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public OrderStatus Status { get; init; }
    public AddressDto DeliveryAddress { get; init; } = new();
    public IReadOnlyList<OrderItemDto> Items { get; init; } = [];
    public decimal Total { get; init; }
}
