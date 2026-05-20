using OnlineShop.Db.Models;

namespace OnlineShop.Db.Interfaces;

public interface IReviewsStorage
{
    Task<IReadOnlyList<Review>> GetByProductAsync(Guid productId, CancellationToken ct = default);

    Task<double> GetAverageRatingAsync(Guid productId, CancellationToken ct = default);

    Task AddAsync(Review review, CancellationToken ct = default);
}
