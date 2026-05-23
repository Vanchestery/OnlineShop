using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Core.Interfaces;
using OnlineShop.Db.Models;
using OnlineShop.Web.ViewModels.Reviews;

namespace OnlineShop.Web.ViewComponents;

/// <summary>
/// Секция отзывов на странице товара: средний рейтинг + список + форма (для авторизованных).
/// Один компонент, который сам решает что показать в зависимости от auth-состояния.
/// </summary>
public class ProductReviewsViewComponent : ViewComponent
{
    private readonly IReviewService _reviewService;
    private readonly UserManager<User> _userManager;

    public ProductReviewsViewComponent(
        IReviewService reviewService,
        UserManager<User> userManager)
    {
        _reviewService = reviewService;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync(Guid productId)
    {
        var items = await _reviewService.GetForProductAsync(productId);
        var avg = await _reviewService.GetAverageRatingAsync(productId);
        var user = await _userManager.GetUserAsync(HttpContext.User);

        return View(new ProductReviewsViewModel
        {
            ProductId = productId,
            AverageRating = avg,
            Count = items.Count,
            IsAuthenticated = user is not null,
            Items = items.Select(ReviewItemViewModel.FromDto).ToList(),
            NewReview = new AddReviewViewModel { ProductId = productId }
        });
    }
}
