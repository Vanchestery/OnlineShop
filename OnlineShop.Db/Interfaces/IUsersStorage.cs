using OnlineShop.Db.Models;

namespace OnlineShop.Db.Interfaces;

/// <summary>
/// Тонкая обёртка для запросов, которых нет в UserManager (joins с заказами/ролями и т.п.).
/// Базовые операции (создание, поиск по email, смена пароля) — через UserManager&lt;User&gt;.
/// </summary>
public interface IUsersStorage
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default);
}
