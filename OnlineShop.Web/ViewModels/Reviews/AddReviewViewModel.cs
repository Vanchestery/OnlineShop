using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Web.ViewModels.Reviews;

public class AddReviewViewModel
{
    /// <summary>
    /// Скрытое поле — id товара, к которому относится отзыв.
    /// </summary>
    public Guid ProductId { get; set; }

    [Required(ErrorMessage = "Оценка обязательна")]
    [Range(1, 5, ErrorMessage = "Оценка от 1 до 5")]
    [Display(Name = "Оценка")]
    public int Rating { get; set; } = 5;

    [Required(ErrorMessage = "Напишите текст отзыва")]
    [StringLength(4000, MinimumLength = 3, ErrorMessage = "От 3 до 4000 символов")]
    [Display(Name = "Текст отзыва")]
    public string Text { get; set; } = string.Empty;
}
