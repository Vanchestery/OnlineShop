using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Db;
using OnlineShop.Db.Models;
using OnlineShop.Web.Areas.Admin.ViewModels;

namespace OnlineShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class RolesController : Controller
{
    private readonly RoleManager<Role> _roleManager;
    private readonly ILogger<RolesController> _logger;

    public RolesController(RoleManager<Role> roleManager, ILogger<RolesController> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var roles = await _roleManager.Roles
            .OrderBy(r => r.Name)
            .Select(r => r.Name!)
            .ToListAsync(ct);
        return View(roles);
    }

    [HttpGet]
    public IActionResult Create() => View(new CreateRoleViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRoleViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _roleManager.RoleExistsAsync(model.Name))
        {
            ModelState.AddModelError(nameof(model.Name), "Роль с таким именем уже существует");
            return View(model);
        }

        var result = await _roleManager.CreateAsync(new Role(model.Name));
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }
            return View(model);
        }

        _logger.LogInformation("Role {Name} created", model.Name);
        TempData["StatusMessage"] = $"Роль «{model.Name}» создана.";
        return RedirectToAction(nameof(Index));
    }
}
