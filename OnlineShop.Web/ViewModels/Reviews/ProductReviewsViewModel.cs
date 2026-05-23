namespace OnlineShop.Web.ViewModels.Reviews;

/// <summary>
/// Контейнерная модель для ProductReviewsViewComponent: средний рейтинг,
/// список отзывов, флаг авторизации (определяет показать форму или ссылку на login).
/// </summary>
public class ProductReviewsViewModel
{
    public Guid ProductId { get; init; }
    public double AverageRating { get; init; }
    public int Count { get; init; }
    public bool IsAuthenticated { get; init; }
    public IReadOnlyList<ReviewItemViewModel> Items { get; init; } = [];
    public AddReviewViewModel NewReview { get; init; } = new();
}
