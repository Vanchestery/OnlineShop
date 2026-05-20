using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShop.Db.Models;

namespace OnlineShop.Db.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Базовая Identity-конфигурация (имена таблиц, индексы Name/NormalizedName)
        // приходит из IdentityDbContext.OnModelCreating. Здесь — только наши добавки.
    }
}
