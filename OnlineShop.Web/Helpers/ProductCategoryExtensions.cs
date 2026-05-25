using OnlineShop.Db.Models;

namespace OnlineShop.Web.Helpers;

/// <summary>
/// UI-локализация ProductCategory. Лежит в Web — Db не должен знать про человеческие названия.
/// </summary>
public static class ProductCategoryExtensions
{
    public static string ToRussian(this ProductCategory category) => category switch
    {
        ProductCategory.Coffee => "Кофе",
        ProductCategory.Tea => "Чай",
        ProductCategory.Accessory => "Аксессуары",
        ProductCategory.Other => "Прочее",
        _ => category.ToString()
    };
}
