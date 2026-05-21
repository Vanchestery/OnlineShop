using OnlineShop.Core.Dtos;
using OnlineShop.Db.Models;

namespace OnlineShop.Web.ViewModels.Auth;

public class OrderHistoryItemViewModel
{
    public Guid Id { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public OrderStatus Status { get; init; }
    public int ItemsCount { get; init; }
    public decimal Total { get; init; }

    public static OrderHistoryItemViewModel FromDto(OrderDto dto) => new()
    {
        Id = dto.Id,
        CreatedAt = dto.CreatedAt,
        Status = dto.Status,
        ItemsCount = dto.Items.Sum(i => i.Quantity),
        Total = dto.Total
    };
}
