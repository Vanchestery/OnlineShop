namespace OnlineShop.Web.Helpers;

/// <summary>
/// Правильная склоняемость русских числительных. Не путать с английским
/// `count == 1 ? "review" : "reviews"` — в русском три формы и есть
/// исключения для чисел 11-14 (даже если оканчиваются на 2,3,4).
///
/// Правила:
/// — оканчивается на 1, кроме 11 → форма "один" (1 отзыв, 21 отзыв)
/// — оканчивается на 2,3,4, кроме 12,13,14 → форма "два" (2 отзыва, 23 отзыва)
/// — иначе → форма "пять" (0 отзывов, 5 отзывов, 11 отзывов, 25 отзывов)
/// </summary>
public static class RussianPluralizer
{
    public static string Pluralize(int count, string one, string few, string many)
    {
        var mod100 = Math.Abs(count) % 100;
        // Исключение: 11-14 всегда форма "many"
        if (mod100 >= 11 && mod100 <= 14) return many;

        var mod10 = mod100 % 10;
        if (mod10 == 1) return one;
        if (mod10 >= 2 && mod10 <= 4) return few;
        return many;
    }

    // Удобные шорткаты для частых случаев

    public static string Reviews(int count) => Pluralize(count, "отзыв", "отзыва", "отзывов");
    public static string Products(int count) => Pluralize(count, "товар", "товара", "товаров");
    public static string Orders(int count) => Pluralize(count, "заказ", "заказа", "заказов");
    public static string Items(int count) => Pluralize(count, "позиция", "позиции", "позиций");
}
