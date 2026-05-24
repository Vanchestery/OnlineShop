using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Core.Interfaces;
using OnlineShop.Db;
using OnlineShop.Db.Models;
using OnlineShop.Web.Areas.Admin.ViewModels;

namespace OnlineShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(OrderStatus? status, CancellationToken ct)
    {
        var orders = await _orderService.GetAllAsync(ct);
        if (status.HasValue)
        {
            orders = orders.Where(o => o.Status == status.Value).ToList();
        }
        ViewData["FilterStatus"] = status;
        return View(orders.Select(OrderRowViewModel.FromDto).ToList());
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var dto = await _orderService.GetByIdAsync(id, ct);
        if (dto is null) return NotFound();
        return View(AdminOrderDetailsViewModel.FromDto(dto));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(Guid id, OrderStatus newStatus, CancellationToken ct)
    {
        try
        {
            await _orderService.ChangeStatusAsync(id, newStatus, ct);
            _logger.LogInformation("Order {OrderId} status changed to {Status}", id, newStatus);
            TempData["StatusMessage"] = $"Статус заказа изменён на «{newStatus}».";
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Change status failed for {OrderId}", id);
            TempData["StatusMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id });
    }
}
