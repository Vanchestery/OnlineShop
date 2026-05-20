using OnlineShop.Db.Models;

namespace OnlineShop.Db.Interfaces;

public interface IProductsStorage
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<Product>> GetAllAsync(bool includeUnavailable = false, CancellationToken ct = default);

    Task<IReadOnlyList<Product>> SearchAsync(string? query, CancellationToken ct = default);

    Task AddAsync(Product product, CancellationToken ct = default);

    Task UpdateAsync(Product product, CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
