using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShop.Db.Models;

namespace OnlineShop.Db.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(4000)
            .IsRequired();

        // decimal(10,2) — до 99 999 999.99. Для магазина уровня pet-проекта достаточно.
        builder.Property(p => p.Price)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(p => p.ImagePath)
            .HasMaxLength(500);

        // ВАЖНО: НЕ ставить HasDefaultValue(ProductCategory.Other) — это ловушка.
        // EF Core тогда считает свойство value-generated, и при INSERT пропускает
        // колонку если значение свойства равно C# default'у (0 для enum). Coffee=0,
        // поэтому Coffee-товары сохранялись с DB-default'ом Other. Бесспорно одна
        // из топовых EF-ловушек.
        builder.Property(p => p.Category).IsRequired();

        // Индекс на имя — для поиска LIKE/ILIKE.
        builder.HasIndex(p => p.Name);

        // Индекс на флаг доступности — частый фильтр в каталоге.
        builder.HasIndex(p => p.IsAvailable);

        // Индекс на категорию — частый фильтр в каталоге.
        builder.HasIndex(p => p.Category);
    }
}
