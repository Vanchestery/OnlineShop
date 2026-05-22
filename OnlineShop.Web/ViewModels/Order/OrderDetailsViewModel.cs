using OnlineShop.Core.Dtos;
using OnlineShop.Db.Models;

namespace OnlineShop.Web.ViewModels.Order;

public class OrderDetailsViewModel
{
    public Guid Id { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public OrderStatus Status { get; init; }
    public AddressViewModel DeliveryAddress { get; init; } = new();
    public IReadOnlyList<OrderDetailsItemViewModel> Items { get; init; } = [];
    public decimal Total { get; init; }

    public static OrderDetailsViewModel FromDto(OrderDto dto) => new()
    {
        Id = dto.Id,
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
