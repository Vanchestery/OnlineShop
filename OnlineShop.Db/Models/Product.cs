namespace OnlineShop.Db.Models;

/// <summary>
/// Товар в каталоге.
/// </summary>
public class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    /// <summary>
    /// Категория товара. Используется для фильтра в каталоге.
    /// </summary>
    public ProductCategory Category { get; set; } = ProductCategory.Other;

    /// <summary>
    /// Относительный путь к картинке (например "/images/products/abc.jpg"),
    /// null если картинки нет.
    /// </summary>
    public string? ImagePath { get; set; }

    /// <summary>
    /// Доступен ли товар к покупке. False — товар "скрыт", но история заказов сохраняется.
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Навигация
    public ICollection<Review> Reviews { get; set; } = [];
}
