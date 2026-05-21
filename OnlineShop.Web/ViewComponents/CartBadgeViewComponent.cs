using Microsoft.AspNetCore.Mvc;
using OnlineShop.Core.Interfaces;
using OnlineShop.Web.Infrastructure;

namespace OnlineShop.Web.ViewComponents;

/// <summary>
/// Рендерит ссылку "Корзина" в навбаре с красным badge'м количества товаров.
/// Запрос лёгкий — только SUM(Quantity), без загрузки полной корзины.
/// </summary>
public class CartBadgeViewComponent : ViewComponent
{
    private readonly ICartContext _cartContext;
    private readonly ICartService _cartService;

    public CartBadgeViewComponent(ICartContext cartContext, ICartService cartService)
    {
        _cartContext = cartContext;
        _cartService = cartService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var cartId = await _cartContext.GetCartIdAsync();
        var count = await _cartService.GetItemCountAsync(cartId);
        return View(count);
    }
}
