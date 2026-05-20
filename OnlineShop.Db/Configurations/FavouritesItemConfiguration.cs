using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShop.Db.Models;

namespace OnlineShop.Db.Configurations;

public class FavouritesItemConfiguration : IEntityTypeConfiguration<FavouritesItem>
{
    public void Configure(EntityTypeBuilder<FavouritesItem> builder)
    {
        builder.HasKey(f => f.Id);

        builder.HasOne(f => f.User)
            .WithMany(u => u.Favourites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.Product)
            .WithMany()
            .HasForeignKey(f => f.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Уникальный индекс — нельзя добавить тот же товар в избранное дважды.
        builder.HasIndex(f => new { f.UserId, f.ProductId }).IsUnique();
    }
}
