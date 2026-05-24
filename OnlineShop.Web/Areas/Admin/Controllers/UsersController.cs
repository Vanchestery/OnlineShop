using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Core.Interfaces;
using OnlineShop.Db;
using OnlineShop.Db.Models;
using OnlineShop.Web.Areas.Admin.ViewModels;

namespace OnlineShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var users = await _userService.GetAllAsync(ct);
        return View(users.Select(UserRowViewModel.FromDto).ToList());
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var dto = await _userService.GetByIdAsync(id, ct);
        if (dto is null) return NotFound();

        var allRoles = _roleManager.Roles.Select(r => r.Name!).OrderBy(n => n).ToList();
        return View(UserDetailsViewModel.FromDto(dto, allRoles));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRoles(Guid id, RolesAssignment rolesAssignment, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();

        var current = await _userManager.GetRolesAsync(user);
        var desired = rolesAssignment.SelectedRoles ?? [];

        var toAdd = desired.Except(current).ToList();
        var toRemove = current.Except(desired).ToList();

        if (toRemove.Count > 0) await _userManager.RemoveFromRolesAsync(user, toRemove);
        if (toAdd.Count > 0) await _userManager.AddToRolesAsync(user, toAdd);

        _logger.LogInformation("Roles changed for {Email}: +{Added}, -{Removed}",
            user.Email, string.Join(",", toAdd), string.Join(",", toRemove));
        TempData["StatusMessage"] = "Роли обновлены.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(Guid id, PasswordReset passwordReset, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            TempData["StatusMessage"] = "Проверь поля пароля.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();

        // Reset через токен — стандартный паттерн админского сброса пароля.
        // RemovePassword + AddPassword тоже сработал бы, но через токен — каноничный
        // путь, который встроен в Identity (используется в "Forgot password" flow).
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, passwordReset.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            TempData["StatusMessage"] = $"Не удалось сменить пароль: {errors}";
        }
        else
        {
            _logger.LogInformation("Password reset by admin for {Email}", user.Email);
            TempData["StatusMessage"] = "Пароль изменён.";
        }
        return RedirectToAction(nameof(Details), new { id });
    }
}
