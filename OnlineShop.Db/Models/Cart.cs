namespace OnlineShop.Db.Models;

/// <summary>
/// Корзина. Может быть пользовательской (UserId != null) или анонимной (UserId == null).
/// Для анонимной корзины Id хранится в куке и так находится при следующих запросах.
/// При логине анонимной корзины делается merge с пользовательской.
/// </summary>
public class Cart
{
    public Guid Id { get; set; }

    /// <summary>
    /// null — анонимная корзина. Иначе — корзина пользователя.
    /// </summary>
    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<CartItem> Items { get; set; } = [];
}
