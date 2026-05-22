using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Core.Interfaces;
using OnlineShop.Db.Models;
using OnlineShop.Web.ViewModels.Favourites;

namespace OnlineShop.Web.Controllers;

[Authorize]
public class FavouriteController : Controller
{
    private readonly IFavouriteService _favouriteService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<FavouriteController> _logger;

    public FavouriteController(
        IFavouriteService favouriteService,
        UserManager<User> userManager,
        ILogger<FavouriteController> logger)
    {
        _favouriteService = favouriteService;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        var items = await _favouriteService.GetForUserAsync(user.Id, ct);
        return View(new FavouriteListViewModel
        {
            Items = items.Select(FavouriteItemViewModel.FromDto).ToList()
        });
    }

    /// <summary>
    /// Один action для add/remove — серверная сторона сама определяет состояние.
    /// Удобно для кнопки-сердечка: одна форма, не нужно знать на клиенте текущий статус.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(Guid productId, string? returnUrl, CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        try
        {
            var isNowFavourite = await _favouriteService.ToggleAsync(user.Id, productId, ct);
            _logger.LogInformation("Favourite toggled product {ProductId} for {UserId} → {State}",
                productId, user.Id, isNowFavourite ? "added" : "removed");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Toggle favourite failed for {ProductId}", productId);
            TempData["StatusMessage"] = ex.Message;
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(Guid productId, CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        await _favouriteService.RemoveAsync(user.Id, productId, ct);
        return RedirectToAction(nameof(Index));
    }
}
