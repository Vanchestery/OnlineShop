using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;

namespace OnlineShop.Db.Storage;

public class FavouritesStorage : IFavouritesStorage
{
    private readonly ApplicationDbContext _db;

    public FavouritesStorage(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<FavouritesItem>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        await _db.Favourites
            .Include(f => f.Product)
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.AddedAt)
            .ToListAsync(ct);

    public Task<bool> IsFavouriteAsync(Guid userId, Guid productId, CancellationToken ct = default) =>
        _db.Favourites.AnyAsync(f => f.UserId == userId && f.ProductId == productId, ct);

    public async Task AddAsync(Guid userId, Guid productId, CancellationToken ct = default)
    {
        if (await IsFavouriteAsync(userId, productId, ct)) return;

        _db.Favourites.Add(new FavouritesItem
        {
            UserId = userId,
            ProductId = productId
        });
        await _db.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Guid userId, Guid productId, CancellationToken ct = default)
    {
        var item = await _db.Favourites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId, ct);
        if (item is null) return;
        _db.Favourites.Remove(item);
        await _db.SaveChangesAsync(ct);
    }
}
