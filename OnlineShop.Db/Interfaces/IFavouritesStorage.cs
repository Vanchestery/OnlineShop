using OnlineShop.Db.Models;

namespace OnlineShop.Db.Interfaces;

public interface IFavouritesStorage
{
    Task<IReadOnlyList<FavouritesItem>> GetByUserAsync(Guid userId, CancellationToken ct = default);

    Task<bool> IsFavouriteAsync(Guid userId, Guid productId, CancellationToken ct = default);

    Task AddAsync(Guid userId, Guid productId, CancellationToken ct = default);

    Task RemoveAsync(Guid userId, Guid productId, CancellationToken ct = default);
}
