using OnlineShop.Core.Dtos;

namespace OnlineShop.Core.Interfaces;

public interface ICartService
{
    Task<CartDto?> GetForUserAsync(Guid userId, CancellationToken ct = default);

    Task<CartDto?> GetByCartIdAsync(Guid cartId, CancellationToken ct = default);

    /// <summary>
    /// Создать анонимную корзину и вернуть её Id (для записи в куку).
    /// </summary>
    Task<Guid> CreateAnonymousAsync(CancellationToken ct = default);

    Task AddItemAsync(Guid cartId, Guid productId, int quantity, CancellationToken ct = default);

    Task DecreaseItemAsync(Guid cartId, Guid productId, CancellationToken ct = default);

    Task RemoveItemAsync(Guid cartId, Guid productId, CancellationToken ct = default);

    Task ClearAsync(Guid cartId, CancellationToken ct = default);

    /// <summary>
    /// Слить анонимную корзину в пользовательскую при логине.
    /// Возвращает Id пользовательской корзины (после мерджа).
    /// Если у юзера ещё нет корзины — создаст её.
    /// </summary>
    Task<Guid> MergeAnonymousIntoUserAsync(Guid anonymousCartId, Guid userId, CancellationToken ct = default);
}
