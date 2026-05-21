using Microsoft.AspNetCore.Mvc;
using OnlineShop.Core.Interfaces;
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
    /// Каталог товаров с опциональным поиском по querystring (?query=...).
    /// </summary>
    public async Task<IActionResult> Index(string? query, CancellationToken ct)
    {
        var dtos = string.IsNullOrWhiteSpace(query)
            ? await _products.GetAllAsync(includeUnavailable: false, ct)
            : await _products.SearchAsync(query, ct);

        var model = new ProductListViewModel
        {
            Query = query,
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
