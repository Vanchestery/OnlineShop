using OnlineShop.Core.Dtos;

namespace OnlineShop.Web.ViewModels.Order;

public class OrderDetailsItemViewModel
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Quantity { get; init; }
    public decimal Subtotal { get; init; }

    public static OrderDetailsItemViewModel FromDto(OrderItemDto dto) => new()
    {
        ProductId = dto.ProductId,
        ProductName = dto.ProductName,
        Price = dto.Price,
        Quantity = dto.Quantity,
        Subtotal = dto.Subtotal
    };
}
