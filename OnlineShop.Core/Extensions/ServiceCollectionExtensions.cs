using Microsoft.Extensions.DependencyInjection;
using OnlineShop.Core.Interfaces;
using OnlineShop.Core.Services;

namespace OnlineShop.Core.Extensions;

/// <summary>
/// Точка входа для регистрации Core-слоя в DI: AutoMapper-профайлы и все сервисы.
/// Program.cs делает: builder.Services.AddCoreLayer();
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreLayer(this IServiceCollection services)
    {
        // Сканируем сборку OnlineShop.Core и регистрируем все Profile-наследники.
        services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly);

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IFavouriteService, FavouriteService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
