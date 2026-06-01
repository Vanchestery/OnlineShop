using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Core.Interfaces;
using OnlineShop.Web.Models;
using OnlineShop.Web.ViewModels.Catalog;

namespace OnlineShop.Web.Controllers;

public class HomeController : Controller
{
    private const int FeaturedCount = 4;

    private readonly IProductService _products;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IProductService products, ILogger<HomeController> logger)
    {
        _products = products;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var all = await _products.GetAllAsync(includeUnavailable: false, ct);
        var featured = all
            .Take(FeaturedCount)
            .Select(ProductCardViewModel.FromDto)
            .ToList();
        return View(featured);
    }

    /// <summary>
    /// Универсальная error-страница для 404 и 500.
    /// — При 404: middleware StatusCodePagesWithReExecute переадресует сюда
    ///   с query-параметром ?statusCode=404.
    /// — При 500: UseExceptionHandler("/Home/Error") выставляет
    ///   HttpContext.Response.StatusCode = 500 перед re-execute, и берётся оттуда.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode)
    {
        var code = statusCode ?? HttpContext.Response.StatusCode;
        if (code < 400) code = 500;  // на случай если status ещё не выставлен

        Response.StatusCode = code;

        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            StatusCode = code
        });
    }
}
