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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
