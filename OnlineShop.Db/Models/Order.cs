namespace OnlineShop.Db.Models;

/// <summary>
/// Заказ. Снимок корзины на момент оформления + адрес доставки + статус.
/// </summary>
public class Order
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public OrderStatus Status { get; set; } = OrderStatus.Created;

    /// <summary>
    /// Адрес доставки. Owned-type — встроен в таблицу Orders как набор колонок,
    /// без отдельной таблицы и FK.
    /// </summary>
    public Address DeliveryAddress { get; set; } = new();

    public ICollection<OrderItem> Items { get; set; } = [];

    /// <summary>
    /// Сумма заказа — считается на лету по позициям. В БД не хранится.
    /// </summary>
    public decimal Total => Items.Sum(i => i.Price * i.Quantity);
}
