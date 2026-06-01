using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Db.Models;

/// <summary>
/// Категория товара. Используется для фильтра в каталоге.
/// [Display(Name)] подхватывается Html.GetEnumSelectList в админ-формах —
/// в select появляются русские названия вместо английских имён enum'а.
/// </summary>
public enum ProductCategory
{
    [Display(Name = "Кофе")]
    Coffee = 0,

    [Display(Name = "Чай")]
    Tea = 1,

    [Display(Name = "Аксессуары")]
    Accessory = 2,

    [Display(Name = "Прочее")]
    Other = 3
}
