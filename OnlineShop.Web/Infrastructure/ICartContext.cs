namespace OnlineShop.Web.Infrastructure;

/// <summary>
/// Знает текущий CartId в контексте запроса: для залогиненного юзера —
/// его корзину, для анонимного — корзину из cookie (создаёт при первом обращении).
/// Результат кэшируется на один request чтобы не делать DB-запрос повторно.
/// </summary>
public interface ICartContext
{
    Task<Guid> GetCartIdAsync(CancellationToken ct = default);
}
