using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Storage;

namespace OnlineShop.Db.Extensions;

/// <summary>
/// Точка входа для регистрации слоя данных в DI: DbContext, хранилища, инициализатор.
/// Identity (AddIdentity) регистрируется в Web-слое, потому что требует
/// shared framework Microsoft.AspNetCore.App (cookies, authentication scheme),
/// который не доступен в обычной class library.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found. " +
                "Add it to appsettings.Development.json or via User Secrets.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));


        // Хранилища
        services.AddScoped<IProductsStorage, ProductsStorage>();
        services.AddScoped<IShoppingCartStorage, ShoppingCartStorage>();
        services.AddScoped<IOrdersStorage, OrdersStorage>();
        services.AddScoped<IFavouritesStorage, FavouritesStorage>();
        services.AddScoped<IReviewsStorage, ReviewsStorage>();
        services.AddScoped<IUsersStorage, UsersStorage>();

        // Инициализатор БД — для вызова из Program.cs
        services.AddScoped<IdentityInitializer>();

        return services;
    }
}
