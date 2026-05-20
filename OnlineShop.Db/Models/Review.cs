namespace OnlineShop.Db.Models;

/// <summary>
/// Отзыв на товар. Rating 1..5, текст обязателен, один отзыв = одна запись.
/// </summary>
public class Review
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    /// <summary>
    /// Оценка от 1 до 5. Check-constraint в БД гарантирует диапазон.
    /// </summary>
    public int Rating { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
