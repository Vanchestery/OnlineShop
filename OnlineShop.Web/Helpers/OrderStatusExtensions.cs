using OnlineShop.Db.Models;

namespace OnlineShop.Web.Helpers;

/// <summary>
/// UI-локализация OrderStatus. Лежит в Web — Db не должен знать про человеческие
/// названия (это responsibility представления).
/// </summary>
public static class OrderStatusExtensions
{
    public static string ToRussian(this OrderStatus status) => status switch
    {
        OrderStatus.Created => "Создан",
        OrderStatus.Paid => "Оплачен",
        OrderStatus.Shipped => "Отправлен",
        OrderStatus.Delivered => "Доставлен",
        OrderStatus.Cancelled => "Отменён",
        _ => status.ToString()
    };

    /// <summary>
    /// CSS-класс bootstrap badge для статуса.
    /// </summary>
    public static string ToBadgeClass(this OrderStatus status) => status switch
    {
        OrderStatus.Created => "bg-secondary",
        OrderStatus.Paid => "bg-info",
        OrderStatus.Shipped => "bg-primary",
        OrderStatus.Delivered => "bg-success",
        OrderStatus.Cancelled => "bg-danger",
        _ => "bg-secondary"
    };
}
