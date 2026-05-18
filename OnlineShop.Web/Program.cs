var builder = WebApplication.CreateBuilder(args);

// MVC: контроллеры + представления. AddRazorRuntimeCompilation добавим позже,
// когда понадобится править .cshtml без перезапуска приложения (Фаза 4).
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Конвейер обработки HTTP-запросов.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS — указание браузеру всегда ходить по HTTPS. По умолчанию 30 дней.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();   // отдача wwwroot

app.UseRouting();

// Authentication/Authorization добавим в Фазе 5 (после подключения Identity).
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
