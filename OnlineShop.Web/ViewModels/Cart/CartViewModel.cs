using OnlineShop.Core.Dtos;

namespace OnlineShop.Web.ViewModels.Cart;

public class CartViewModel
{
    public Guid Id { get; init; }
    public IReadOnlyList<CartItemViewModel> Items { get; init; } = [];
    public decimal Total { get; init; }
    public bool IsEmpty => Items.Count == 0;

    public static CartViewModel FromDto(CartDto dto) => new()
    {
        Id = dto.Id,
        Items = dto.Items.Select(CartItemViewModel.FromDto).ToList(),
        Total = dto.Total
    };
}
