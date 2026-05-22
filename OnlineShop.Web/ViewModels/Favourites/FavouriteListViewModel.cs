namespace OnlineShop.Web.ViewModels.Favourites;

public class FavouriteListViewModel
{
    public IReadOnlyList<FavouriteItemViewModel> Items { get; init; } = [];
    public bool IsEmpty => Items.Count == 0;
}
