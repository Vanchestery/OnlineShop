using OnlineShop.Db.Models;

namespace OnlineShop.Db.Interfaces;

/// <summary>
/// Одно хранилище на обе разновидности корзин — пользовательскую (Cart.UserId != null)
/// и анонимную (Cart.UserId == null). Различие — только в способе нахождения корзины:
/// по UserId или по CartId из куки.
/// </summary>
public interface IShoppingCartStorage
{
    Task<Cart?> GetByIdAsync(Guid cartId, CancellationToken ct = default);

    Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

    Task<Cart> GetOrCreateForUserAsync(Guid userId, CancellationToken ct = default);

    Task<Cart> CreateAnonymousAsync(CancellationToken ct = default);

    /// <summary>
    /// Быстрый подсчёт SUM(Quantity) без загрузки сущностей —
    /// для бейджа корзины в навбаре. 0 если корзины нет или она пуста.
    /// </summary>
    Task<int> GetItemCountAsync(Guid cartId, CancellationToken ct = default);

    Task AddItemAsync(Guid cartId, Guid productId, int quantity, CancellationToken ct = default);

    Task DecreaseItemAsync(Guid cartId, Guid productId, CancellationToken ct = default);

    Task RemoveItemAsync(Guid cartId, Guid productId, CancellationToken ct = default);

    Task ClearAsync(Guid cartId, CancellationToken ct = default);

    /// <summary>
    /// Перенести позиции из source-корзины в target и удалить source.
    /// Используется при логине: анонимная корзина → пользовательская.
    /// </summary>
    Task MergeAsync(Guid sourceCartId, Guid targetCartId, CancellationToken ct = default);

    Task DeleteAsync(Guid cartId, CancellationToken ct = default);
}
