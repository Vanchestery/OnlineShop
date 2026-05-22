namespace OnlineShop.Web.ViewModels.Favourites;

/// <summary>
/// Модель для FavouriteButton ViewComponent. Три состояния: аноним, в избранном, не в избранном.
/// </summary>
public class FavouriteButtonViewModel
{
    public Guid ProductId { get; init; }
    public bool IsAuthenticated { get; init; }
    public bool IsFavourite { get; init; }
}
