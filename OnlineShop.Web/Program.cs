using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Core.Extensions;
using OnlineShop.Db;
using OnlineShop.Db.Extensions;
using OnlineShop.Db.Models;

var builder = WebApplication.CreateBuilder(args);

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

app.UseRouting();

// Порядок важен: Authentication до Authorization.
// Authentication устанавливает HttpContext.User, Authorization его проверяет.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
