using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;

namespace OnlineShop.Db.Storage;

public class OrdersStorage : IOrdersStorage
{
    private readonly ApplicationDbContext _db;

    public OrdersStorage(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IReadOnlyList<Order>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        await _db.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Orders
            .Include(o => o.Items)
            .Include(o => o.User)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(Order order, CancellationToken ct = default)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken ct = default)
    {
        var order = await _db.Orders.FindAsync([id], ct)
            ?? throw new InvalidOperationException($"Order {id} not found.");
        order.Status = status;
        await _db.SaveChangesAsync(ct);
    }
}
