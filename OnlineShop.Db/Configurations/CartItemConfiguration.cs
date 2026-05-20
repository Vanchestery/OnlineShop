using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShop.Db.Models;

namespace OnlineShop.Db.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(ci => ci.Id);

        // CartItem принадлежит Cart. Удаление корзины удаляет позиции.
        builder.HasOne(ci => ci.Cart)
            .WithMany(c => c.Items)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        // CartItem ссылается на Product. Restrict — товар нельзя удалить пока он
        // в чьей-то корзине. На практике admin использует soft-delete через
        // Product.IsAvailable=false; при оформлении заказа сервис проверяет
        // IsAvailable и отказывает в покупке "скрытых" товаров.
        builder.HasOne(ci => ci.Product)
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Quantity должно быть > 0. CHECK-constraint на уровне БД.
        builder.ToTable(t => t.HasCheckConstraint("CK_CartItem_Quantity", "\"Quantity\" > 0"));
    }
}
