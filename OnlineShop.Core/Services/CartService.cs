using AutoMapper;
using OnlineShop.Core.Dtos;
using OnlineShop.Core.Interfaces;
using OnlineShop.Db.Interfaces;

namespace OnlineShop.Core.Services;

public class CartService : ICartService
{
    private readonly IShoppingCartStorage _carts;
    private readonly IProductsStorage _products;
    private readonly IMapper _mapper;

    public CartService(
        IShoppingCartStorage carts,
        IProductsStorage products,
        IMapper mapper)
    {
        _carts = carts;
        _products = products;
        _mapper = mapper;
    }

    public async Task<CartDto?> GetForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var cart = await _carts.GetByUserIdAsync(userId, ct);
        return cart is null ? null : _mapper.Map<CartDto>(cart);
    }

    public async Task<CartDto?> GetByCartIdAsync(Guid cartId, CancellationToken ct = default)
    {
        var cart = await _carts.GetByIdAsync(cartId, ct);
        return cart is null ? null : _mapper.Map<CartDto>(cart);
    }

    public async Task<Guid> CreateAnonymousAsync(CancellationToken ct = default)
    {
        var cart = await _carts.CreateAnonymousAsync(ct);
        return cart.Id;
    }

    public async Task AddItemAsync(Guid cartId, Guid productId, int quantity, CancellationToken ct = default)
    {
        if (quantity <= 0)
        {
            throw new InvalidOperationException("Количество должно быть положительным.");
        }

        var product = await _products.GetByIdAsync(productId, ct)
            ?? throw new InvalidOperationException($"Товар {productId} не найден.");

        if (!product.IsAvailable)
        {
            throw new InvalidOperationException($"Товар «{product.Name}» недоступен к покупке.");
        }

        await _carts.AddItemAsync(cartId, productId, quantity, ct);
    }

    public Task DecreaseItemAsync(Guid cartId, Guid productId, CancellationToken ct = default) =>
        _carts.DecreaseItemAsync(cartId, productId, ct);

    public Task RemoveItemAsync(Guid cartId, Guid productId, CancellationToken ct = default) =>
        _carts.RemoveItemAsync(cartId, productId, ct);

    public Task ClearAsync(Guid cartId, CancellationToken ct = default) =>
        _carts.ClearAsync(cartId, ct);

    public async Task<Guid> MergeAnonymousIntoUserAsync(Guid anonymousCartId, Guid userId, CancellationToken ct = default)
    {
        // Целевая корзина — пользовательская. Создаём, если ещё нет.
        var userCart = await _carts.GetOrCreateForUserAsync(userId, ct);

        // Если идентификаторы совпадают (странный edge case) — нечего сливать.
        if (anonymousCartId == userCart.Id) return userCart.Id;

        // Проверим что source существует (может быть просрочена/удалена).
        var source = await _carts.GetByIdAsync(anonymousCartId, ct);
        if (source is null) return userCart.Id;

        // Запретим случайный merge пользовательской корзины в другую пользовательскую.
        if (source.UserId is not null)
        {
            throw new InvalidOperationException("Source-корзина не анонимна — merge не делается.");
        }

        await _carts.MergeAsync(anonymousCartId, userCart.Id, ct);
        return userCart.Id;
    }
}
