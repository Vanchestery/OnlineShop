using OnlineShop.Db.Models;

namespace OnlineShop.Web.ViewModels.Catalog;

/// <summary>
/// Каталог с поиском и фильтром по категории. Query и Category сохраняются
/// чтобы перерисовать активное состояние в форме/тегах.
/// </summary>
public class ProductListViewModel
{
    public string? Query { get; init; }
    public ProductCategory? Category { get; init; }
    public IReadOnlyList<ProductCardViewModel> Items { get; init; } = [];
    public int TotalCount => Items.Count;
}
