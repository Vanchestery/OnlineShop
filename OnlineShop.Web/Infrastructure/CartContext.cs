using Microsoft.AspNetCore.Identity;
using OnlineShop.Core.Interfaces;
using OnlineShop.Db.Models;

namespace OnlineShop.Web.Infrastructure;

public class CartContext : ICartContext
{
    /// <summary>
    /// Имя cookie, в которой живёт Id анонимной корзины.
    /// Доступно AccountController для удаления cookie после merge.
    /// </summary>
    public const string CookieName = "shop_cart";

    private static readonly TimeSpan CookieLifetime = TimeSpan.FromDays(30);

    private readonly IHttpContextAccessor _httpContext;
    private readonly UserManager<User> _userManager;
    private readonly ICartService _cartService;

    // Кэш на один request — scoped lifetime гарантирует что один CartContext
    // живёт один запрос.
    private Guid? _cached;

    public CartContext(
        IHttpContextAccessor httpContext,
        UserManager<User> userManager,
        ICartService cartService)
    {
        _httpContext = httpContext;
        _userManager = userManager;
        _cartService = cartService;
    }

    public async Task<Guid> GetCartIdAsync(CancellationToken ct = default)
    {
        if (_cached.HasValue) return _cached.Value;

        var ctx = _httpContext.HttpContext
            ?? throw new InvalidOperationException("CartContext can be used only inside HTTP request.");

        Guid id;
        if (ctx.User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(ctx.User);
            if (user is not null)
            {
                id = await _cartService.GetOrCreateForUserAsync(user.Id, ct);
            }
            else
            {
                // edge case: кука авторизации жива, но юзера в БД нет (удалён админом и т.п.)
                id = await ResolveAnonymousAsync(ctx, ct);
            }
        }
        else
        {
            id = await ResolveAnonymousAsync(ctx, ct);
        }

        _cached = id;
        return id;
    }

    private async Task<Guid> ResolveAnonymousAsync(HttpContext ctx, CancellationToken ct)
    {
        // Если кука есть и распарсилась как Guid — доверяем ей.
        // Если корзина была удалена (например, merge'нута и cookie не очищена) —
        // следующая операция Add создаст новую через middleware при следующем заходе.
        // На pet-проекте это приемлемо. В проде стоит проверять существование.
        if (ctx.Request.Cookies.TryGetValue(CookieName, out var raw)
            && Guid.TryParse(raw, out var existing))
        {
            return existing;
        }

        var newId = await _cartService.CreateAnonymousAsync(ct);
        ctx.Response.Cookies.Append(CookieName, newId.ToString(), new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.Add(CookieLifetime),
            HttpOnly = true,             // JS не может прочитать — защита от XSS
            SameSite = SameSiteMode.Lax, // отправляется при top-level navigation, но не на cross-site requests
            IsEssential = true,          // обходит cookie consent prompt (это essential для функциональности)
            Secure = ctx.Request.IsHttps // только по HTTPS, кроме локального http
        });
        return newId;
    }
}
