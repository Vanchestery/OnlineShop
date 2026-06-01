using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Core.Dtos;
using OnlineShop.Core.Interfaces;
using OnlineShop.Db;
using OnlineShop.Web.Areas.Admin.ViewModels;

namespace OnlineShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class ProductsController : Controller
{
    private const long MaxImageBytes = 5 * 1024 * 1024;  // 5 MB
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    private readonly IProductService _productService;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        IWebHostEnvironment env,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _env = env;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var products = await _productService.GetAllAsync(includeUnavailable: true, ct);
        return View(products.Select(ProductRowViewModel.FromDto).ToList());
    }

    [HttpGet]
    public IActionResult Create() => View(new ProductFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            var imagePath = await SaveImageAsync(model.ImageFile);
            var dto = new ProductDto
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Category = model.Category,
                IsAvailable = model.IsAvailable,
                ImagePath = imagePath
            };
            await _productService.CreateAsync(dto, ct);
            TempData["StatusMessage"] = $"Товар «{model.Name}» создан.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var dto = await _productService.GetByIdAsync(id, ct);
        if (dto is null) return NotFound();
        return View(ProductFormViewModel.FromDto(dto));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductFormViewModel model, CancellationToken ct)
    {
        if (model.Id is null) return NotFound();
        if (!ModelState.IsValid) return View(model);

        try
        {
            var newImagePath = await SaveImageAsync(model.ImageFile);
            var imagePath = newImagePath ?? model.ExistingImagePath;
            var dto = new ProductDto
            {
                Id = model.Id.Value,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Category = model.Category,
                IsAvailable = model.IsAvailable,
                ImagePath = imagePath
            };
            await _productService.UpdateAsync(model.Id.Value, dto, ct);

            // Если admin загрузил новую картинку — старая стала orphan'ом, удаляем.
            if (newImagePath is not null
                && !string.IsNullOrEmpty(model.ExistingImagePath)
                && model.ExistingImagePath != newImagePath)
            {
                TryDeleteImageFile(model.ExistingImagePath);
            }
            TempData["StatusMessage"] = "Товар обновлён.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        // Получаем DTO перед удалением — нужен путь к картинке для cleanup.
        var dto = await _productService.GetByIdAsync(id, ct);

        try
        {
            await _productService.DeleteAsync(id, ct);

            // Удаляем физический файл картинки с диска. Делаем после успешного
            // DeleteAsync — если БД отказала по FK, файл не трогаем.
            if (dto is not null && !string.IsNullOrEmpty(dto.ImagePath))
            {
                TryDeleteImageFile(dto.ImagePath);
            }

            TempData["StatusMessage"] = "Товар удалён.";
        }
        catch (Exception ex)
        {
            // Скорее всего FK-violation (товар есть в OrderItem). Сообщаем юзеру,
            // что нужно использовать soft-delete через IsAvailable=false.
            _logger.LogWarning(ex, "Hard delete product {Id} failed", id);
            TempData["StatusMessage"] = "Товар нельзя удалить — он есть в заказах. " +
                                        "Откройте редактирование и снимите галочку «Доступен к покупке».";
        }
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Физическое удаление файла из wwwroot/. Тихо ловим ошибки —
    /// orphaned-файл не катастрофа, основная операция уже прошла.
    /// </summary>
    private void TryDeleteImageFile(string relativePath)
    {
        var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/'));
        try
        {
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                _logger.LogInformation("Deleted image file {Path}", fullPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete image file {Path}", fullPath);
        }
    }

    /// <summary>
    /// Сохраняет загруженный файл в wwwroot/images/products/. Возвращает относительный URL
    /// или null если файл не пришёл. Валидирует расширение и размер.
    /// </summary>
    private async Task<string?> SaveImageAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0) return null;

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
        {
            throw new InvalidOperationException(
                $"Допустимые форматы: {string.Join(", ", AllowedExtensions)}");
        }
        if (file.Length > MaxImageBytes)
        {
            throw new InvalidOperationException(
                $"Размер файла не должен превышать {MaxImageBytes / 1024 / 1024} MB");
        }

        var fileName = $"{Guid.NewGuid()}{ext}";
        var dir = Path.Combine(_env.WebRootPath, "images", "products");
        Directory.CreateDirectory(dir);
        var filePath = Path.Combine(dir, fileName);

        await using var stream = System.IO.File.Create(filePath);
        await file.CopyToAsync(stream);

        return $"/images/products/{fileName}";
    }
}
