using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Models;

namespace OnlineShop.Db;

/// <summary>
/// Один контекст на всё приложение — наследуется от IdentityDbContext, поэтому таблицы
/// AspNetUsers / AspNetRoles / AspNetUserRoles / AspNetUserClaims / AspNetRoleClaims /
/// AspNetUserLogins / AspNetUserTokens создаются автоматически. Наши доменные таблицы
/// — DbSet&lt;...&gt; ниже.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<FavouritesItem> Favourites => Set<FavouritesItem>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Сначала — Identity-конфигурация (имена таблиц AspNet*, индексы и т.д.).
        base.OnModelCreating(modelBuilder);

        // Затем — все наши IEntityTypeConfiguration<T> из этой сборки.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
