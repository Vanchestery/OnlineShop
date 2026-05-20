using OnlineShop.Core.Dtos;

namespace OnlineShop.Core.Interfaces;

public interface IFavouriteService
{
    Task<IReadOnlyList<FavouriteItemDto>> GetForUserAsync(Guid userId, CancellationToken ct = default);

    Task<bool> IsFavouriteAsync(Guid userId, Guid productId, CancellationToken ct = default);

    Task AddAsync(Guid userId, Guid productId, CancellationToken ct = default);

    Task RemoveAsync(Guid userId, Guid productId, CancellationToken ct = default);

    /// <summary>
    /// Если товар в избранном — удалить, если нет — добавить.
    /// Возвращает true, если после операции товар в избранном.
    /// </summary>
    Task<bool> ToggleAsync(Guid userId, Guid productId, CancellationToken ct = default);
}
