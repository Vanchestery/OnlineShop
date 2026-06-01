namespace OnlineShop.Web.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    /// <summary>
    /// HTTP-статус (404, 500, ...). Заполняется контроллером либо из query string
    /// (когда StatusCodePagesWithReExecute переадресовал сюда 404), либо из
    /// HttpContext.Response.StatusCode (когда UseExceptionHandler перехватил 500).
    /// </summary>
    public int StatusCode { get; set; } = 500;

    public string Title => StatusCode switch
    {
        404 => "Страница не найдена",
        403 => "Доступ запрещён",
        500 => "Что-то пошло не так",
        _ => "Что-то пошло не так"
    };

    public string Description => StatusCode switch
    {
        404 => "Возможно, ссылка устарела или вы ошиблись адресом",
        403 => "У вас нет прав для просмотра этой страницы",
        500 => "На сервере произошла ошибка — мы уже знаем",
        _ => "Произошла непредвиденная ошибка"
    };
}
