using AutoMapper;
using OnlineShop.Core.Services;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;

namespace OnlineShop.Tests.Services;

public class CartServiceTests
{
    private readonly Mock<IShoppingCartStorage> _carts = new();
    private readonly Mock<IProductsStorage> _products = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly CartService _sut;

    public CartServiceTests()
    {
        _sut = new CartService(_carts.Object, _products.Object, _mapper.Object);
    }

    // ─── MergeAnonymousIntoUserAsync ─────────────────────────────────

    [Fact]
    public async Task MergeAnonymous_HappyPath_CallsStorageMerge()
    {
        var userId = Guid.NewGuid();
        var anonCartId = Guid.NewGuid();
        var userCart = new Cart { Id = Guid.NewGuid(), UserId = userId };
        var anonCart = new Cart { Id = anonCartId, UserId = null };

        _carts.Setup(c => c.GetOrCreateForUserAsync(userId, It.IsAny<CancellationToken>()))
              .ReturnsAsync(userCart);
        _carts.Setup(c => c.GetByIdAsync(anonCartId, It.IsAny<CancellationToken>()))
              .ReturnsAsync(anonCart);

        var resultId = await _sut.MergeAnonymousIntoUserAsync(anonCartId, userId);

        resultId.Should().Be(userCart.Id);
        _carts.Verify(
            c => c.MergeAsync(anonCartId, userCart.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task MergeAnonymous_SourceCartMissing_NoOpReturnsUserCartId()
    {
        // Кука с просроченным/удалённым cartId — не падаем, просто игнорируем.
        var userId = Guid.NewGuid();
        var anonCartId = Guid.NewGuid();
        var userCart = new Cart { Id = Guid.NewGuid(), UserId = userId };

        _carts.Setup(c => c.GetOrCreateForUserAsync(userId, It.IsAny<CancellationToken>()))
              .ReturnsAsync(userCart);
        _carts.Setup(c => c.GetByIdAsync(anonCartId, It.IsAny<CancellationToken>()))
              .ReturnsAsync((Cart?)null);

        var resultId = await _sut.MergeAnonymousIntoUserAsync(anonCartId, userId);

        resultId.Should().Be(userCart.Id);
        _carts.Verify(
            c => c.MergeAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task MergeAnonymous_SourceNotAnonymous_ThrowsInvalidOperation()
    {
        // Защита: не позволяем merge'ить чужую пользовательскую корзину
        // (например при spoof'е куки или баге).
        var userId = Guid.NewGuid();
        var anonCartId = Guid.NewGuid();
        var someOtherUserId = Guid.NewGuid();
        var userCart = new Cart { Id = Guid.NewGuid(), UserId = userId };
        var notAnonymousCart = new Cart { Id = anonCartId, UserId = someOtherUserId };

        _carts.Setup(c => c.GetOrCreateForUserAsync(userId, It.IsAny<CancellationToken>()))
              .ReturnsAsync(userCart);
        _carts.Setup(c => c.GetByIdAsync(anonCartId, It.IsAny<CancellationToken>()))
              .ReturnsAsync(notAnonymousCart);

        var act = () => _sut.MergeAnonymousIntoUserAsync(anonCartId, userId);

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*не анонимна*");
    }

    [Fact]
    public async Task MergeAnonymous_SameCartId_ShortCircuits()
    {
        // Edge case — если как-то совпали Id, не делаем ничего лишнего.
        var userId = Guid.NewGuid();
        var cartId = Guid.NewGuid();
        var userCart = new Cart { Id = cartId, UserId = userId };

        _carts.Setup(c => c.GetOrCreateForUserAsync(userId, It.IsAny<CancellationToken>()))
              .ReturnsAsync(userCart);

        var resultId = await _sut.MergeAnonymousIntoUserAsync(cartId, userId);

        resultId.Should().Be(cartId);
        _carts.Verify(
            c => c.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _carts.Verify(
            c => c.MergeAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ─── AddItemAsync ────────────────────────────────────────────────

    [Fact]
    public async Task AddItem_ProductNotFound_ThrowsInvalidOperation()
    {
        var productId = Guid.NewGuid();
        _products.Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Product?)null);

        var act = () => _sut.AddItemAsync(Guid.NewGuid(), productId, 1);

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*не найден*");
    }

    [Fact]
    public async Task AddItem_UnavailableProduct_ThrowsInvalidOperation()
    {
        var productId = Guid.NewGuid();
        var unavailable = new Product { Id = productId, Name = "Test", IsAvailable = false };

        _products.Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(unavailable);

        var act = () => _sut.AddItemAsync(Guid.NewGuid(), productId, 1);

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*недоступен*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task AddItem_NonPositiveQuantity_ThrowsInvalidOperation(int quantity)
    {
        var act = () => _sut.AddItemAsync(Guid.NewGuid(), Guid.NewGuid(), quantity);

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*положительным*");
    }

    [Fact]
    public async Task AddItem_AvailableProduct_DelegatesToStorage()
    {
        var cartId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Test", IsAvailable = true };

        _products.Setup(p => p.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(product);

        await _sut.AddItemAsync(cartId, productId, 3);

        _carts.Verify(
            c => c.AddItemAsync(cartId, productId, 3, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
