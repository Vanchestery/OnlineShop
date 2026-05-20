using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using OnlineShop.Db.Storage;

namespace OnlineShop.Db.Extensions;

/// <summary>
/// Точка входа для регистрации всего слоя данных в DI.
/// Program.cs делает один вызов: builder.Services.AddDataLayer(builder.Configuration).
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

        // Параметры паролей и юзеров — мягкие для pet-проекта.
        services.AddIdentity<User, Role>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

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
