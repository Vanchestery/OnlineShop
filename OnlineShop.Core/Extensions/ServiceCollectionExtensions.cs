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
        // В AutoMapper 14+ убрали перегрузку AddAutoMapper(Assembly), оставили только
        // с Action<IMapperConfigurationExpression>. Внутри явно вызываем AddMaps(asm).
        // Этот синтаксис работает И в 13.x, и в 14.x — backwards-compatible.
        services.AddAutoMapper(cfg =>
            cfg.AddMaps(typeof(ServiceCollectionExtensions).Assembly));

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IFavouriteService, FavouriteService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
