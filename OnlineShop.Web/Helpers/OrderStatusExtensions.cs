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
    /// CSS-класс брутал-бейджа для статуса.
    /// Используется вместе с базовым классом .brutal-badge:
    ///   &lt;span class="brutal-badge @status.ToBadgeClass()"&gt;@status.ToRussian()&lt;/span&gt;
    /// </summary>
    public static string ToBadgeClass(this OrderStatus status) => status switch
    {
        OrderStatus.Created   => "brutal-badge-gray",
        OrderStatus.Paid      => "brutal-badge-lime",
        OrderStatus.Shipped   => "brutal-badge-accent",
        OrderStatus.Delivered => "brutal-badge-success",
        OrderStatus.Cancelled => "brutal-badge-orange",
        _ => "brutal-badge-gray"
    };
}
