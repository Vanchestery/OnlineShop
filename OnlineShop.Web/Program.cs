using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Core.Extensions;
using OnlineShop.Db;
using OnlineShop.Db.Extensions;
using OnlineShop.Db.Models;
using Serilog;
using Serilog.Formatting.Compact;

// Serilog Bootstrap-логгер — пишет до того как DI готов (на случай ошибок старта).
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Запуск приложения");

    var builder = WebApplication.CreateBuilder(args);

    // Подменяем встроенный ILogger на Serilog. JSON-файл с дневной ротацией,
    // лежит в logs/ рядом с .exe (bin/Debug/net9.0/logs/...), а не в папке проекта.
    // AppContext.BaseDirectory → директория где лежит исполняемая сборка.
    var logsPath = Path.Combine(AppContext.BaseDirectory, "logs", "log-.json");
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            formatter: new CompactJsonFormatter(),
            path: logsPath,
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 14));

    builder.Services.AddControllersWithViews();

    // Слой данных: DbContext, хранилища, IdentityInitializer (без Identity-cookies).
    builder.Services.AddDataLayer(builder.Configuration);

    // Слой бизнес-логики: AutoMapper + сервисы.
    builder.Services.AddCoreLayer();

    // Identity-регистрация — на Web-слое, потому что использует shared framework
    // (cookies, authentication scheme) который недоступен в class library.
    // Параметры паролей и юзеров — мягкие для pet-проекта.
    builder.Services.AddIdentity<User, Role>(options =>
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

    var app = builder.Build();

    // Применяем миграции и сидим данные при старте.
    // Для dev — удобно: запустил, всё уже есть. В проде так делать не стоит —
    // миграции должны накатываться отдельной командой/CI-job.
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        try
        {
            var db = services.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync();

            var initializer = services.GetRequiredService<IdentityInitializer>();
            await initializer.InitializeAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при инициализации БД при старте приложения");
            throw;
        }
    }

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // HSTS — указание браузеру всегда ходить по HTTPS. По умолчанию 30 дней.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    // Serilog Request Logging — компактный лог по каждому HTTP-запросу
    // (статус, время выполнения, маршрут).
    app.UseSerilogRequestLogging();

    app.UseRouting();

    // Порядок важен: Authentication до Authorization.
    // Authentication устанавливает HttpContext.User, Authorization его проверяет.
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Приложение упало при старте");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
