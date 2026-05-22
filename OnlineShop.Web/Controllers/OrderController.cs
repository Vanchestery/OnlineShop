using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Core.Dtos.Requests;
using OnlineShop.Core.Interfaces;
using OnlineShop.Db.Models;
using OnlineShop.Web.Infrastructure;
using OnlineShop.Web.ViewModels.Cart;
using OnlineShop.Web.ViewModels.Order;

namespace OnlineShop.Web.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;
    private readonly ICartContext _cartContext;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<OrderController> _logger;

    public OrderController(
        IOrderService orderService,
        ICartService cartService,
        ICartContext cartContext,
        UserManager<User> userManager,
        ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _cartService = cartService;
        _cartContext = cartContext;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Checkout(CancellationToken ct)
    {
        var cartId = await _cartContext.GetCartIdAsync(ct);
        var cart = await _cartService.GetByCartIdAsync(cartId, ct);
        if (cart is null || cart.Items.Count == 0)
        {
            TempData["StatusMessage"] = "Корзина пуста.";
            return RedirectToAction("Index", "Cart");
        }

        return View(new CheckoutViewModel
        {
            Cart = CartViewModel.FromDto(cart),
            Address = new AddressViewModel()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model, CancellationToken ct)
    {
        var cartId = await _cartContext.GetCartIdAsync(ct);
        var cart = await _cartService.GetByCartIdAsync(cartId, ct);
        if (cart is null || cart.Items.Count == 0)
        {
            TempData["StatusMessage"] = "Корзина пуста.";
            return RedirectToAction("Index", "Cart");
        }

        // Подкладываем cart обратно для рендера если валидация упадёт.
        model.Cart = CartViewModel.FromDto(cart);

        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        try
        {
            var request = new CreateOrderRequest
            {
                DeliveryAddress = model.Address.ToDto()
            };
            var order = await _orderService.CreateFromCartAsync(user.Id, cartId, request, ct);
            _logger.LogInformation("Order {OrderId} created for {UserId}", order.Id, user.Id);

            TempData["StatusMessage"] = $"Заказ оформлен! Номер: {order.Id.ToString()[..8]}";
            return RedirectToAction(nameof(Details), new { id = order.Id });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            _logger.LogWarning(ex, "Checkout failed for cart {CartId}", cartId);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var order = await _orderService.GetByIdAsync(id, ct);
        if (order is null) return NotFound();

        // Защита: видеть свой заказ может только владелец.
        // (Админ увидит заказы через свою Admin Area в Phase 9 — отдельный путь.)
        var user = await _userManager.GetUserAsync(User);
        if (user is null || order.UserId != user.Id) return Forbid();

        return View(OrderDetailsViewModel.FromDto(order));
    }
}
