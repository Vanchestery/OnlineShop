namespace OnlineShop.Db.Models;

/// <summary>
/// Запись в избранном — связка пользователь ↔ товар.
/// Unique-index по (UserId, ProductId) защищает от дублей.
/// </summary>
public class FavouritesItem
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public DateTimeOffset AddedAt { get; set; } = DateTimeOffset.UtcNow;
}
