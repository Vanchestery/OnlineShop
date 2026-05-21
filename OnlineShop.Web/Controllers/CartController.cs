using Microsoft.AspNetCore.Mvc;
using OnlineShop.Core.Interfaces;
using OnlineShop.Web.Infrastructure;
using OnlineShop.Web.ViewModels.Cart;

namespace OnlineShop.Web.Controllers;

/// <summary>
/// Корзина — работает и для залогиненных, и для анонимных пользователей
/// через ICartContext, который инкапсулирует резолв cartId из user/cookie.
/// </summary>
public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly ICartContext _cartContext;
    private readonly ILogger<CartController> _logger;

    public CartController(
        ICartService cartService,
        ICartContext cartContext,
        ILogger<CartController> logger)
    {
        _cartService = cartService;
        _cartContext = cartContext;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var cartId = await _cartContext.GetCartIdAsync(ct);
        var cart = await _cartService.GetByCartIdAsync(cartId, ct);
        var model = cart is null
            ? new CartViewModel { Id = cartId }
            : CartViewModel.FromDto(cart);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(Guid productId, int quantity, string? returnUrl, CancellationToken ct)
    {
        if (quantity <= 0) quantity = 1;

        var cartId = await _cartContext.GetCartIdAsync(ct);
        try
        {
            await _cartService.AddItemAsync(cartId, productId, quantity, ct);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Не удалось добавить товар {ProductId} в корзину {CartId}", productId, cartId);
            TempData["StatusMessage"] = ex.Message;
        }
        return SafeRedirect(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Decrease(Guid productId, string? returnUrl, CancellationToken ct)
    {
        var cartId = await _cartContext.GetCartIdAsync(ct);
        await _cartService.DecreaseItemAsync(cartId, productId, ct);
        return SafeRedirect(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(Guid productId, string? returnUrl, CancellationToken ct)
    {
        var cartId = await _cartContext.GetCartIdAsync(ct);
        await _cartService.RemoveItemAsync(cartId, productId, ct);
        return SafeRedirect(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear(CancellationToken ct)
    {
        var cartId = await _cartContext.GetCartIdAsync(ct);
        await _cartService.ClearAsync(cartId, ct);
        return RedirectToAction(nameof(Index));
    }

    private IActionResult SafeRedirect(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction(nameof(Index));
    }
}
