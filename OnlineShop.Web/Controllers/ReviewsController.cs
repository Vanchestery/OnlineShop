using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Core.Dtos.Requests;
using OnlineShop.Core.Interfaces;
using OnlineShop.Db.Models;
using OnlineShop.Web.ViewModels.Reviews;

namespace OnlineShop.Web.Controllers;

[Authorize]
public class ReviewsController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(
        IReviewService reviewService,
        UserManager<User> userManager,
        ILogger<ReviewsController> logger)
    {
        _reviewService = reviewService;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(
        [Bind(Prefix = nameof(ProductReviewsViewModel.NewReview))] AddReviewViewModel model,
        CancellationToken ct)
    {
        // Форма embedded в Product/Details — при ошибке нет своей view для редисплея,
        // поэтому возвращаемся на страницу товара с TempData-сообщением.
        if (!ModelState.IsValid)
        {
            var firstError = ModelState.Values
                .SelectMany(v => v.Errors)
                .FirstOrDefault()?.ErrorMessage
                ?? "Ошибка валидации отзыва";
            TempData["StatusMessage"] = firstError;
            return RedirectToAction("Details", "Product", new { id = model.ProductId });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        try
        {
            await _reviewService.AddAsync(model.ProductId, user.Id, new AddReviewRequest
            {
                Rating = model.Rating,
                Text = model.Text
            }, ct);
            _logger.LogInformation("Review added for {ProductId} by {UserId}", model.ProductId, user.Id);
            TempData["StatusMessage"] = "Спасибо за отзыв!";
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Add review failed for {ProductId}", model.ProductId);
            TempData["StatusMessage"] = ex.Message;
        }

        return RedirectToAction("Details", "Product", new { id = model.ProductId });
    }
}
