namespace OnlineShop.Db.Models;

/// <summary>
/// Жизненный цикл заказа.
/// </summary>
public enum OrderStatus
{
    Created = 0,
    Paid = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}
