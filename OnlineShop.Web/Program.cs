using OnlineShop.Db;
using OnlineShop.Db.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Слой данных: DbContext, Identity, хранилища, IdentityInitializer.
builder.Services.AddDataLayer(builder.Configuration);

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
