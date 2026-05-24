using Microsoft.AspNetCore.Mvc;

namespace OnlineShop.Web.Areas.Admin.ViewComponents;

/// <summary>
/// Сайдбар админ-панели с подсветкой текущего раздела.
/// Определяет активный пункт по имени controller'а из RouteData.
/// </summary>
public class LeftMenuViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var currentController = ViewContext.RouteData.Values["controller"]?.ToString() ?? string.Empty;
        // Каст к object обязателен — иначе перегрузка View(string viewName)
        // принимает значение как имя view, а не как модель. Без object cast
        // получаем "view 'Products' not found".
        return View((object)currentController);
    }
}
