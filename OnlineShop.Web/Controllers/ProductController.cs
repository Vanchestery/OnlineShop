using Microsoft.AspNetCore.Mvc;
using OnlineShop.Core.Interfaces;
using OnlineShop.Db.Models;
using OnlineShop.Web.ViewModels.Catalog;

namespace OnlineShop.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _products;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService products, ILogger<ProductController> logger)
    {
        _products = products;
        _logger = logger;
    }

    /// <summary>
    /// Каталог с опциональными query (поиск) и category (фильтр).
    /// </summary>
    public async Task<IActionResult> Index(string? query, ProductCategory? category, CancellationToken ct)
    {
        // SearchAsync обрабатывает оба фильтра + IsAvailable. Если query пустой и category null —
        // вернёт все доступные товары.
        var dtos = await _products.SearchAsync(query, category, ct);

        var model = new ProductListViewModel
        {
            Query = query,
            Category = category,
            Items = dtos.Select(ProductCardViewModel.FromDto).ToList()
        };
        return View(model);
    }

    /// <summary>
    /// Страница товара по Id (Guid в URL).
    /// </summary>
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var dto = await _products.GetByIdAsync(id, ct);
        if (dto is null)
        {
            _logger.LogWarning("Product {Id} not found", id);
            return NotFound();
        }
        return View(ProductDetailsViewModel.FromDto(dto));
    }
}
