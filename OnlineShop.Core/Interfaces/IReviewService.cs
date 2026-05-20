using OnlineShop.Core.Dtos;
using OnlineShop.Core.Dtos.Requests;

namespace OnlineShop.Core.Interfaces;

public interface IReviewService
{
    Task<IReadOnlyList<ReviewDto>> GetForProductAsync(Guid productId, CancellationToken ct = default);

    /// <summary>
    /// Средний рейтинг по товару. 0 если отзывов нет.
    /// </summary>
    Task<double> GetAverageRatingAsync(Guid productId, CancellationToken ct = default);

    /// <summary>
    /// Добавить отзыв. Валидация: Rating в диапазоне [1, 5], Text не пустой.
    /// </summary>
    Task<ReviewDto> AddAsync(Guid productId, Guid userId, AddReviewRequest request, CancellationToken ct = default);
}
