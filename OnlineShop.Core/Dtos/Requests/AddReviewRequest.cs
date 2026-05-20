namespace OnlineShop.Core.Dtos.Requests;

public record AddReviewRequest
{
    public int Rating { get; init; }
    public string Text { get; init; } = string.Empty;
}
