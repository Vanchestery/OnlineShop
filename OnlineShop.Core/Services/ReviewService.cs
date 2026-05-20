using AutoMapper;
using OnlineShop.Core.Dtos;
using OnlineShop.Core.Dtos.Requests;
using OnlineShop.Core.Interfaces;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;

namespace OnlineShop.Core.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewsStorage _reviews;
    private readonly IProductsStorage _products;
    private readonly IMapper _mapper;

    public ReviewService(
        IReviewsStorage reviews,
        IProductsStorage products,
        IMapper mapper)
    {
        _reviews = reviews;
        _products = products;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ReviewDto>> GetForProductAsync(Guid productId, CancellationToken ct = default)
    {
        var entities = await _reviews.GetByProductAsync(productId, ct);
        return _mapper.Map<List<ReviewDto>>(entities);
    }

    public Task<double> GetAverageRatingAsync(Guid productId, CancellationToken ct = default) =>
        _reviews.GetAverageRatingAsync(productId, ct);

    public async Task<ReviewDto> AddAsync(Guid productId, Guid userId, AddReviewRequest request, CancellationToken ct = default)
    {
        if (request.Rating < 1 || request.Rating > 5)
        {
            throw new InvalidOperationException("Оценка должна быть от 1 до 5.");
        }
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            throw new InvalidOperationException("Текст отзыва не может быть пустым.");
        }

        // Убеждаемся что товар существует (а не пытаемся добавить отзыв к призраку).
        _ = await _products.GetByIdAsync(productId, ct)
            ?? throw new InvalidOperationException($"Товар {productId} не найден.");

        var review = new Review
        {
            ProductId = productId,
            UserId = userId,
            Rating = request.Rating,
            Text = request.Text.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };
        await _reviews.AddAsync(review, ct);

        return _mapper.Map<ReviewDto>(review);
    }
}
