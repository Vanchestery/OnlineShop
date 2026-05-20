namespace OnlineShop.Core.Dtos;

public record ReviewDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public int Rating { get; init; }
    public string Text { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
}
