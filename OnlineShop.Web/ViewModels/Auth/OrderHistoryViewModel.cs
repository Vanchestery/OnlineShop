namespace OnlineShop.Web.ViewModels.Auth;

public class OrderHistoryViewModel
{
    public IReadOnlyList<OrderHistoryItemViewModel> Orders { get; init; } = [];
}
