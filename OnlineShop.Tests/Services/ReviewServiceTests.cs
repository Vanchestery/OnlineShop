using AutoMapper;
using OnlineShop.Core.Dtos;
using OnlineShop.Core.Dtos.Requests;
using OnlineShop.Core.Services;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;

namespace OnlineShop.Tests.Services;

public class ReviewServiceTests
{
    private readonly Mock<IReviewsStorage> _reviews = new();
    private readonly Mock<IProductsStorage> _products = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly ReviewService _sut;

    public ReviewServiceTests()
    {
        _sut = new ReviewService(_reviews.Object, _products.Object, _mapper.Object);
        _mapper.Setup(m => m.Map<ReviewDto>(It.IsAny<Review>()))
               .Returns(new ReviewDto());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(100)]
    public async Task Add_RatingOutOfRange_ThrowsInvalidOperation(int rating)
    {
        var act = () => _sut.AddAsync(
            Guid.NewGuid(), Guid.NewGuid(),
            new AddReviewRequest { Rating = rating, Text = "Good" });

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*1 до 5*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task Add_EmptyOrWhitespaceText_ThrowsInvalidOperation(string text)
    {
        var act = () => _sut.AddAsync(
            Guid.NewGuid(), Guid.NewGuid(),
            new AddReviewRequest { Rating = 5, Text = text });

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*пуст*");
    }

    [Fact]
    public async Task Add_ProductNotFound_ThrowsInvalidOperation()
    {
        var productId = Guid.NewGuid();
        _products.Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Product?)null);

        var act = () => _sut.AddAsync(
            productId, Guid.NewGuid(),
            new AddReviewRequest { Rating = 4, Text = "Good" });

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*не найден*");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task Add_ValidInput_PersistsCorrectly(int rating)
    {
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _products.Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new Product { Id = productId, Name = "Test" });

        await _sut.AddAsync(productId, userId,
            new AddReviewRequest { Rating = rating, Text = "Good product" });

        _reviews.Verify(r => r.AddAsync(It.Is<Review>(rev =>
            rev.ProductId == productId
            && rev.UserId == userId
            && rev.Rating == rating
            && rev.Text == "Good product"
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Add_TextWithSurroundingWhitespace_TrimsBeforeStoring()
    {
        var productId = Guid.NewGuid();
        _products.Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new Product { Id = productId });

        await _sut.AddAsync(productId, Guid.NewGuid(),
            new AddReviewRequest { Rating = 5, Text = "  Полный отзыв с пробелами по краям  " });

        _reviews.Verify(r => r.AddAsync(It.Is<Review>(rev =>
            rev.Text == "Полный отзыв с пробелами по краям"
        ), It.IsAny<CancellationToken>()), Times.Once);
    }
}
