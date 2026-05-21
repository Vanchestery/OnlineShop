namespace OnlineShop.Web.ViewModels.Catalog;

/// <summary>
/// Каталог с поиском. Query сохраняется чтобы перерисовать в поле поиска.
/// </summary>
public class ProductListViewModel
{
    public string? Query { get; init; }
    public IReadOnlyList<ProductCardViewModel> Items { get; init; } = [];
    public int TotalCount => Items.Count;
}
