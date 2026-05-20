using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShop.Db.Models;

namespace OnlineShop.Db.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        // Статус как int (0..4). Можно было через HasConversion(string),
        // тогда в БД будет "Created"/"Paid"/... — читаемее, но шире.
        // Оставляем int для компактности.
        builder.Property(o => o.Status).IsRequired();

        // Owned-type для адреса доставки. Создаст колонки
        // DeliveryAddress_Country, DeliveryAddress_City, ... в таблице Orders.
        builder.OwnsOne(o => o.DeliveryAddress, address =>
        {
            address.Property(a => a.Country).HasMaxLength(100).IsRequired();
            address.Property(a => a.City).HasMaxLength(100).IsRequired();
            address.Property(a => a.Street).HasMaxLength(200).IsRequired();
            address.Property(a => a.ZipCode).HasMaxLength(20).IsRequired();
            address.Property(a => a.Apartment).HasMaxLength(50);
        });

        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Сумма Total в БД не хранится — это computed на лету в C#.
        builder.Ignore(o => o.Total);

        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.Status);
    }
}
