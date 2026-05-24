using OnlineShop.Core.Dtos;
using OnlineShop.Db.Models;
using OnlineShop.Web.ViewModels.Order;

namespace OnlineShop.Web.Areas.Admin.ViewModels;

/// <summary>
/// Расширенная детальная страница заказа для админа: всё что видит юзер + кто заказчик,
/// плюс форма смены статуса.
/// </summary>
public class AdminOrderDetailsViewModel
{
    public Guid Id { get; init; }
    public string UserFullName { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public OrderStatus Status { get; init; }
    public AddressViewModel DeliveryAddress { get; init; } = new();
    public IReadOnlyList<OrderDetailsItemViewModel> Items { get; init; } = [];
    public decimal Total { get; init; }

    public static AdminOrderDetailsViewModel FromDto(OrderDto dto) => new()
    {
        Id = dto.Id,
        UserFullName = string.IsNullOrWhiteSpace(dto.UserFullName) ? "—" : dto.UserFullName,
        UserId = dto.UserId,
        CreatedAt = dto.CreatedAt,
        Status = dto.Status,
        DeliveryAddress = new AddressViewModel
        {
            Country = dto.DeliveryAddress.Country,
            City = dto.DeliveryAddress.City,
            Street = dto.DeliveryAddress.Street,
            ZipCode = dto.DeliveryAddress.ZipCode,
            Apartment = dto.DeliveryAddress.Apartment
        },
        Items = dto.Items.Select(OrderDetailsItemViewModel.FromDto).ToList(),
        Total = dto.Total
    };
}
