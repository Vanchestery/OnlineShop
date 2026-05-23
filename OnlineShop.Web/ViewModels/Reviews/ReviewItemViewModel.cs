using OnlineShop.Core.Dtos;

namespace OnlineShop.Web.ViewModels.Reviews;

public class ReviewItemViewModel
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public int Rating { get; init; }
    public string Text { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }

    public static ReviewItemViewModel FromDto(ReviewDto dto) => new()
    {
        Id = dto.Id,
        UserName = string.IsNullOrWhiteSpace(dto.UserName) ? "Аноним" : dto.UserName,
        Rating = dto.Rating,
        Text = dto.Text,
        CreatedAt = dto.CreatedAt
    };
}
