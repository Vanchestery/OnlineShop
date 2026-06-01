using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Db.Models;

/// <summary>
/// Жизненный цикл заказа.
/// [Display(Name)] подхватывается Html.GetEnumSelectList в форме смены статуса —
/// admin видит "Создан/Оплачен/..." вместо "Created/Paid/...".
/// </summary>
public enum OrderStatus
{
    [Display(Name = "Создан")]
    Created = 0,

    [Display(Name = "Оплачен")]
    Paid = 1,

    [Display(Name = "Отправлен")]
    Shipped = 2,

    [Display(Name = "Доставлен")]
    Delivered = 3,

    [Display(Name = "Отменён")]
    Cancelled = 4
}
