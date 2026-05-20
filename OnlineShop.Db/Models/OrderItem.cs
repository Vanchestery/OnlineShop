namespace OnlineShop.Db.Models;

/// <summary>
/// Позиция заказа. Содержит "замороженный" снимок товара на момент оформления:
/// Name и Price копируются в OrderItem, чтобы если потом товар переименуют
/// или изменят цену — в старых заказах сохранилось то, что было в чеке.
/// </summary>
public class OrderItem
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public Guid ProductId { get; set; }
    /// <summary>
    /// Навигация nullable для безопасности (без Include на запросах — будет null).
    /// FK настроен на Restrict — товар нельзя hard-удалить если на него ссылаются
    /// заказы. В админке используется soft-delete через Product.IsAvailable = false.
    /// </summary>
    public Product? Product { get; set; }

    /// <summary>
    /// Снимок названия товара на момент покупки.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Снимок цены товара на момент покупки.
    /// </summary>
    public decimal Price { get; set; }

    public int Quantity { get; set; }
}
