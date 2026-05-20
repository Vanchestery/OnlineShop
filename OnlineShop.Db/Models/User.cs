using Microsoft.AspNetCore.Identity;

namespace OnlineShop.Db.Models;

/// <summary>
/// Пользователь сайта. Наследуется от IdentityUser&lt;Guid&gt; — Identity-таблица AspNetUsers
/// получает Guid как PK и наши дополнительные поля (FirstName, LastName).
/// </summary>
public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Навигация — одна корзина на юзера (1:0..1), много заказов, много избранного, много отзывов.
    public Cart? Cart { get; set; }
    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<FavouritesItem> Favourites { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}
