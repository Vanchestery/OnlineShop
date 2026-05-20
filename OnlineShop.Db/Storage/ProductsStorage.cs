using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;

namespace OnlineShop.Db.Storage;

public class ProductsStorage : IProductsStorage
{
    private readonly ApplicationDbContext _db;

    public ProductsStorage(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Product>> GetAllAsync(bool includeUnavailable = false, CancellationToken ct = default)
    {
        var query = _db.Products.AsNoTracking();
        if (!includeUnavailable)
        {
            query = query.Where(p => p.IsAvailable);
        }
        return await query.OrderBy(p => p.Name).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Product>> SearchAsync(string? query, CancellationToken ct = default)
    {
        var products = _db.Products.AsNoTracking().Where(p => p.IsAvailable);

        if (!string.IsNullOrWhiteSpace(query))
        {
            // ILike — case-insensitive LIKE в PostgreSQL. EF.Functions.ILike транслируется в SQL ILIKE.
            var pattern = $"%{query.Trim()}%";
            products = products.Where(p =>
                EF.Functions.ILike(p.Name, pattern) ||
                EF.Functions.ILike(p.Description, pattern));
        }

        return await products.OrderBy(p => p.Name).ToListAsync(ct);
    }

    public async Task AddAsync(Product product, CancellationToken ct = default)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        product.UpdatedAt = DateTimeOffset.UtcNow;
        _db.Products.Update(product);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await _db.Products.FindAsync([id], ct);
        if (product is null) return;
        _db.Products.Remove(product);
        await _db.SaveChangesAsync(ct);
    }
}
