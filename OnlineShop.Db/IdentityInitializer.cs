using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnlineShop.Db.Models;

namespace OnlineShop.Db;

/// <summary>
/// Стартовая инициализация БД: создание ролей, дефолтного admin-аккаунта, сидинг товаров.
/// Идемпотентна — повторный запуск ничего не ломает.
/// Запускается из Program.cs через app.Services.CreateScope() при старте приложения.
/// </summary>
public class IdentityInitializer
{
    // Хардкод для dev — в проде эти креды должны браться из User Secrets / env vars.
    private const string AdminEmail = "admin@onlineshop.local";
    private const string AdminPassword = "Admin#123";

    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<IdentityInitializer> _logger;

    public IdentityInitializer(
        RoleManager<Role> roleManager,
        UserManager<User> userManager,
        ApplicationDbContext db,
        ILogger<IdentityInitializer> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _db = db;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        await EnsureRolesAsync(ct);
        await EnsureAdminAsync(ct);
        await SeedProductsAsync(ct);
    }

    private async Task EnsureRolesAsync(CancellationToken ct)
    {
        foreach (var roleName in new[] { RoleNames.Admin, RoleNames.User })
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new Role(roleName));
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
                _logger.LogInformation("Created role {Role}", roleName);
            }
        }
    }

    private async Task EnsureAdminAsync(CancellationToken ct)
    {
        var admin = await _userManager.FindByEmailAsync(AdminEmail);
        if (admin is not null) return;

        admin = new User
        {
            UserName = AdminEmail,
            Email = AdminEmail,
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "Default"
        };

        var createResult = await _userManager.CreateAsync(admin, AdminPassword);
        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to create admin: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
        }

        var roleResult = await _userManager.AddToRoleAsync(admin, RoleNames.Admin);
        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to assign admin role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
        }

        _logger.LogInformation("Created admin user {Email}", AdminEmail);
    }

    private async Task SeedProductsAsync(CancellationToken ct)
    {
        if (await _db.Products.AnyAsync(ct)) return;

        var now = DateTimeOffset.UtcNow;
        var products = new List<Product>
        {
            // ─── COFFEE (5) ────────────────────────────────────────────────
            new() { Name = "Кофе зерновой Arabica 1кг", Description = "Высокогорная арабика средней обжарки. Ноты шоколада и карамели. Универсальный профиль на каждый день.", Price = 1290m, Category = ProductCategory.Coffee, ImagePath = "/images/products/coffee-arabica.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },
            new() { Name = "Кофе Ethiopia Yirgacheffe 250г", Description = "Specialty-кофе из Эфиопии. Цветочные ноты, цитрусовая кислинка, светлая обжарка.", Price = 980m, Category = ProductCategory.Coffee, ImagePath = "/images/products/coffee-ethiopia.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },
            new() { Name = "Кофе Colombia Supremo 250г", Description = "Сбалансированный профиль с тонами карамели и ореха. Средняя обжарка.", Price = 890m, Category = ProductCategory.Coffee, ImagePath = "/images/products/coffee-colombia.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },
            new() { Name = "Кофе Kenya AA Top 250г", Description = "Яркая ягодная кислотность, ноты грейпфрута и чёрной смородины. Тёмная обжарка.", Price = 1190m, Category = ProductCategory.Coffee, ImagePath = "/images/products/coffee-kenya.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },
            new() { Name = "Кофе Brazil Santos 250г", Description = "Мягкий вкус с шоколадными и ореховыми оттенками. Классическая бразильская обжарка.", Price = 750m, Category = ProductCategory.Coffee, ImagePath = "/images/products/coffee-brazil.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },

            // ─── TEA (3) ───────────────────────────────────────────────────
            new() { Name = "Чай зелёный Sencha 100г", Description = "Японский зелёный чай первого сбора. Свежий травяной вкус, лёгкий аромат свежескошенной травы.", Price = 580m, Category = ProductCategory.Tea, ImagePath = "/images/products/tea-sencha.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },
            new() { Name = "Чай Улун Те Гуань Инь 100г", Description = "Полуферментированный китайский чай. Ноты орхидеи и персика, высокогорный.", Price = 880m, Category = ProductCategory.Tea, ImagePath = "/images/products/tea-oolong.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },
            new() { Name = "Чай Earl Grey 100г", Description = "Классический английский чай: цейлонская основа с маслом бергамота. Британская традиция.", Price = 540m, Category = ProductCategory.Tea, ImagePath = "/images/products/tea-earl-grey.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },

            // ─── ACCESSORY (9) ─────────────────────────────────────────────
            new() { Name = "Чайник заварочный 800мл", Description = "Стеклянный заварник с фильтром из нержавеющей стали. Подходит для любого вида чая.", Price = 1450m, Category = ProductCategory.Accessory, ImagePath = "/images/products/accessory-teapot.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },
            new() { Name = "Кружка керамическая 350мл", Description = "Матовая глазурь, удобная ручка, посудомоечная машина — да.", Price = 390m, Category = ProductCategory.Accessory, ImagePath = "/images/products/accessory-mug.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },
            new() { Name = "Кофемолка ручная", Description = "Бурр-механизм с регулировкой помола. 6 ступеней — от эспрессо до френч-пресса.", Price = 3490m, Category = ProductCategory.Accessory, ImagePath = "/images/products/accessory-grinder.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },
            new() { Name = "Френч-пресс 600мл", Description = "Боросиликатное стекло, двойной фильтр, металлическое основание.", Price = 1190m, Category = ProductCategory.Accessory, ImagePath = "/images/products/accessory-french-press.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },
            new() { Name = "Молочник металлический 350мл", Description = "Для взбивания молока и капучино. Носик для латте-арта.", Price = 690m, Category = ProductCategory.Accessory, ImagePath = "/images/products/accessory-milk-jug.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },
            new() { Name = "Термокружка 500мл", Description = "Нержавейка, держит температуру до 8 часов. Крышка с защитой от протекания.", Price = 1990m, Category = ProductCategory.Accessory, ImagePath = "/images/products/accessory-thermo-mug.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },
            new() { Name = "Подставка для кружек деревянная", Description = "Натуральный дуб, лазерная гравировка. Набор из 4 штук.", Price = 560m, Category = ProductCategory.Accessory, ImagePath = "/images/products/accessory-coaster.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },
            new() { Name = "Турка медная 500мл", Description = "Медная турка для приготовления кофе по-восточному. На 3-4 чашки.", Price = 1290m, Category = ProductCategory.Accessory, ImagePath = "/images/products/accessory-cezve.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },
            new() { Name = "Дрипер V60 керамический", Description = "Pour-over дрипер. Спиральные рёбра внутри улучшают экстракцию.", Price = 1990m, Category = ProductCategory.Accessory, ImagePath = "/images/products/accessory-v60.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },

            // ─── OTHER (3) ─────────────────────────────────────────────────
            new() { Name = "Шоколад тёмный 70% 100г", Description = "Бельгийский шоколад без добавок. Хорошо подходит к кофе и крепкому чаю.", Price = 220m, Category = ProductCategory.Other, ImagePath = "/images/products/other-chocolate.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },
            new() { Name = "Печенье миндальное 200г", Description = "Хрустящее печенье с миндальной мукой. Без глютена.", Price = 320m, Category = ProductCategory.Other, ImagePath = "/images/products/other-cookies.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },
            new() { Name = "Книга «Кофе. От зерна к чашке»", Description = "Иллюстрированное руководство по обжарке, помолу и приготовлению. 240 страниц.", Price = 890m, Category = ProductCategory.Other, ImagePath = "/images/products/other-book.jpg", IsAvailable = true, CreatedAt = now, UpdatedAt = now },
        };

        _db.Products.AddRange(products);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Seeded {Count} demo products", products.Count);
    }
}
