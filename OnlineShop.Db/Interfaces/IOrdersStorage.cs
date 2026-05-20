using OnlineShop.Db.Models;

namespace OnlineShop.Db.Interfaces;

public interface IOrdersStorage
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<Order>> GetByUserAsync(Guid userId, CancellationToken ct = default);

    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default);

    Task AddAsync(Order order, CancellationToken ct = default);

    Task UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken ct = default);
}
