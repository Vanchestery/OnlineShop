using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Core.Interfaces;
using OnlineShop.Db.Models;
using OnlineShop.Web.ViewModels.Favourites;

namespace OnlineShop.Web.ViewComponents;

/// <summary>
/// Рендерит кнопку-сердечко для конкретного товара. Три состояния:
/// 1) аноним → outline-серая, клик ведёт на login
/// 2) залогинен, не в избранном → outline-чёрная "♡", клик добавляет
/// 3) залогинен, в избранном → красная заполненная "❤", клик удаляет
/// </summary>
public class FavouriteButtonViewComponent : ViewComponent
{
    private readonly UserManager<User> _userManager;
    private readonly IFavouriteService _favouriteService;

    public FavouriteButtonViewComponent(
        UserManager<User> userManager,
        IFavouriteService favouriteService)
    {
        _userManager = userManager;
        _favouriteService = favouriteService;
    }

    public async Task<IViewComponentResult> InvokeAsync(Guid productId)
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        var isFavourite = user is not null
            && await _favouriteService.IsFavouriteAsync(user.Id, productId);

        return View(new FavouriteButtonViewModel
        {
            ProductId = productId,
            IsAuthenticated = user is not null,
            IsFavourite = isFavourite
        });
    }
}
