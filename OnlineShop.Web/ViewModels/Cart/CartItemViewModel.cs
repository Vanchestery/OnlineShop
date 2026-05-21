using OnlineShop.Core.Dtos;

namespace OnlineShop.Web.ViewModels.Cart;

public class CartItemViewModel
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal ProductPrice { get; init; }
    public string? ProductImagePath { get; init; }
    public int Quantity { get; init; }
    public decimal Subtotal { get; init; }

    public static CartItemViewModel FromDto(CartItemDto dto) => new()
    {
        ProductId = dto.ProductId,
        ProductName = dto.ProductName,
        ProductPrice = dto.ProductPrice,
        ProductImagePath = dto.ProductImagePath,
        Quantity = dto.Quantity,
        Subtotal = dto.Subtotal
    };
}
