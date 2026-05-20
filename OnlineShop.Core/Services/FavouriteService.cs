using AutoMapper;
using OnlineShop.Core.Dtos;
using OnlineShop.Core.Interfaces;
using OnlineShop.Db.Interfaces;

namespace OnlineShop.Core.Services;

public class FavouriteService : IFavouriteService
{
    private readonly IFavouritesStorage _favourites;
    private readonly IProductsStorage _products;
    private readonly IMapper _mapper;

    public FavouriteService(
        IFavouritesStorage favourites,
        IProductsStorage products,
        IMapper mapper)
    {
        _favourites = favourites;
        _products = products;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<FavouriteItemDto>> GetForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var entities = await _favourites.GetByUserAsync(userId, ct);
        return _mapper.Map<List<FavouriteItemDto>>(entities);
    }

    public Task<bool> IsFavouriteAsync(Guid userId, Guid productId, CancellationToken ct = default) =>
        _favourites.IsFavouriteAsync(userId, productId, ct);

    public async Task AddAsync(Guid userId, Guid productId, CancellationToken ct = default)
    {
        var product = await _products.GetByIdAsync(productId, ct)
            ?? throw new InvalidOperationException($"Товар {productId} не найден.");

        if (!product.IsAvailable)
        {
            throw new InvalidOperationException($"Товар «{product.Name}» недоступен.");
        }

        await _favourites.AddAsync(userId, productId, ct);
    }

    public Task RemoveAsync(Guid userId, Guid productId, CancellationToken ct = default) =>
        _favourites.RemoveAsync(userId, productId, ct);

    public async Task<bool> ToggleAsync(Guid userId, Guid productId, CancellationToken ct = default)
    {
        var isFav = await _favourites.IsFavouriteAsync(userId, productId, ct);
        if (isFav)
        {
            await _favourites.RemoveAsync(userId, productId, ct);
            return false;
        }
        else
        {
            await AddAsync(userId, productId, ct);  // через AddAsync — заодно проверит IsAvailable
            return true;
        }
    }
}
