using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;

namespace OnlineShop.Db.Storage;

public class ReviewsStorage : IReviewsStorage
{
    private readonly ApplicationDbContext _db;

    public ReviewsStorage(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Review>> GetByProductAsync(Guid productId, CancellationToken ct = default) =>
        await _db.Reviews
            .Include(r => r.User)
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

    public async Task<double> GetAverageRatingAsync(Guid productId, CancellationToken ct = default)
    {
        // Если отзывов нет — AverageAsync кинет InvalidOperationException на пустой последовательности.
        // Поэтому вычисляем через Where + AnyAsync, либо проще — Sum/Count вручную.
        var ratings = _db.Reviews.Where(r => r.ProductId == productId).Select(r => (double)r.Rating);
        var any = await ratings.AnyAsync(ct);
        if (!any) return 0.0;
        return await ratings.AverageAsync(ct);
    }

    public async Task AddAsync(Review review, CancellationToken ct = default)
    {
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync(ct);
    }
}
