using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;

namespace OnlineShop.Db.Storage;

public class ShoppingCartStorage : IShoppingCartStorage
{
    private readonly ApplicationDbContext _db;

    public ShoppingCartStorage(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<Cart?> GetByIdAsync(Guid cartId, CancellationToken ct = default) =>
        _db.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.Id == cartId, ct);

    public Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        _db.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

    public async Task<Cart> GetOrCreateForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var cart = await GetByUserIdAsync(userId, ct);
        if (cart is not null) return cart;

        cart = new Cart { UserId = userId };
        _db.Carts.Add(cart);
        await _db.SaveChangesAsync(ct);
        return cart;
    }

    public async Task<Cart> CreateAnonymousAsync(CancellationToken ct = default)
    {
        var cart = new Cart { UserId = null };
        _db.Carts.Add(cart);
        await _db.SaveChangesAsync(ct);
        return cart;
    }

    public Task<int> GetItemCountAsync(Guid cartId, CancellationToken ct = default) =>
        _db.CartItems
            .Where(i => i.CartId == cartId)
            .SumAsync(i => i.Quantity, ct);
    // SumAsync на пустой выборке возвращает 0 — для int-селектора это поведение EF
    // (в SQL SUM(NULL) = NULL, но EF приводит к 0). Удобно — не надо коалесить.

    public async Task AddItemAsync(Guid cartId, Guid productId, int quantity, CancellationToken ct = default)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");

        var cart = await _db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == cartId, ct)
            ?? throw new InvalidOperationException($"Cart {cartId} not found.");

        var existing = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is null)
        {
            cart.Items.Add(new CartItem
            {
                ProductId = productId,
                Quantity = quantity
            });
        }
        else
        {
            existing.Quantity += quantity;
        }

        cart.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task DecreaseItemAsync(Guid cartId, Guid productId, CancellationToken ct = default)
    {
        var item = await _db.CartItems
            .FirstOrDefaultAsync(i => i.CartId == cartId && i.ProductId == productId, ct);
        if (item is null) return;

        if (item.Quantity > 1)
        {
            item.Quantity--;
        }
        else
        {
            _db.CartItems.Remove(item);
        }
        await _db.SaveChangesAsync(ct);
    }

    public async Task RemoveItemAsync(Guid cartId, Guid productId, CancellationToken ct = default)
    {
        var item = await _db.CartItems
            .FirstOrDefaultAsync(i => i.CartId == cartId && i.ProductId == productId, ct);
        if (item is null) return;
        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync(ct);
    }

    public async Task ClearAsync(Guid cartId, CancellationToken ct = default)
    {
        var items = await _db.CartItems.Where(i => i.CartId == cartId).ToListAsync(ct);
        _db.CartItems.RemoveRange(items);
        await _db.SaveChangesAsync(ct);
    }

    public async Task MergeAsync(Guid sourceCartId, Guid targetCartId, CancellationToken ct = default)
    {
        if (sourceCartId == targetCartId) return;

        var source = await _db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == sourceCartId, ct);
        if (source is null) return;

        var target = await _db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == targetCartId, ct)
            ?? throw new InvalidOperationException($"Target cart {targetCartId} not found.");

        foreach (var srcItem in source.Items)
        {
            var existing = target.Items.FirstOrDefault(i => i.ProductId == srcItem.ProductId);
            if (existing is null)
            {
                target.Items.Add(new CartItem
                {
                    ProductId = srcItem.ProductId,
                    Quantity = srcItem.Quantity
                });
            }
            else
            {
                existing.Quantity += srcItem.Quantity;
            }
        }

        _db.Carts.Remove(source);
        target.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid cartId, CancellationToken ct = default)
    {
        var cart = await _db.Carts.FindAsync([cartId], ct);
        if (cart is null) return;
        _db.Carts.Remove(cart);
        await _db.SaveChangesAsync(ct);
    }
}
