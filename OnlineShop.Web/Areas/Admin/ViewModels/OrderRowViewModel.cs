using OnlineShop.Core.Dtos;
using OnlineShop.Db.Models;

namespace OnlineShop.Web.Areas.Admin.ViewModels;

public class OrderRowViewModel
{
    public Guid Id { get; init; }
    public string UserFullName { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public OrderStatus Status { get; init; }
    public int ItemsCount { get; init; }
    public decimal Total { get; init; }

    public static OrderRowViewModel FromDto(OrderDto dto) => new()
    {
        Id = dto.Id,
        UserFullName = string.IsNullOrWhiteSpace(dto.UserFullName) ? "—" : dto.UserFullName,
        CreatedAt = dto.CreatedAt,
        Status = dto.Status,
        ItemsCount = dto.Items.Sum(i => i.Quantity),
        Total = dto.Total
    };
}
