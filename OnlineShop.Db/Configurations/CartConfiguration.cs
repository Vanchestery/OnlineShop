using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShop.Db.Models;

namespace OnlineShop.Db.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(c => c.Id);

        // Уникальный индекс на UserId, но только когда UserId не null —
        // у одного пользователя максимум одна корзина, анонимных может быть много.
        // PostgreSQL-специфичный partial index: HasFilter с SQL-выражением.
        builder.HasIndex(c => c.UserId)
            .IsUnique()
            .HasFilter("\"UserId\" IS NOT NULL");

        // Связь с User описана в UserConfiguration (HasOne(u => u.Cart).WithOne(...)).
        // Связь с CartItem — в CartItemConfiguration.
    }
}
