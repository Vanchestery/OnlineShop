using OnlineShop.Web.ViewModels.Cart;

namespace OnlineShop.Web.ViewModels.Order;

public class CheckoutViewModel
{
    /// <summary>
    /// Адрес доставки — биндится из формы. mutable class с DataAnnotations.
    /// </summary>
    public AddressViewModel Address { get; set; } = new();

    /// <summary>
    /// Снимок корзины для отображения в правой колонке формы (read-only).
    /// Заполняется контроллером перед рендером.
    /// </summary>
    public CartViewModel Cart { get; set; } = new();
}
