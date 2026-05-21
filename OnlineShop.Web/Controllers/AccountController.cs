using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Core.Interfaces;
using OnlineShop.Db;
using OnlineShop.Db.Models;
using OnlineShop.Web.Infrastructure;
using OnlineShop.Web.ViewModels.Auth;

namespace OnlineShop.Web.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IOrderService orderService,
        ICartService cartService,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _orderService = orderService;
        _cartService = cartService;
        _logger = logger;
    }

    // ─── Регистрация ───────────────────────────────────────────────────

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new User
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            EmailConfirmed = true,  // для dev — без email-подтверждения
            CreatedAt = DateTimeOffset.UtcNow
        };

        var create = await _userManager.CreateAsync(user, model.Password);
        if (!create.Succeeded)
        {
            foreach (var err in create.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }
            return View(model);
        }

        // Всех новых юзеров — в роль User. Admin-роль выдаётся только из админки.
        await _userManager.AddToRoleAsync(user, RoleNames.User);
        await _signInManager.SignInAsync(user, isPersistent: false);

        // Если в анонимной куке была корзина с товарами — перенесём в пользовательскую.
        await TryMergeAnonymousCartAsync(user.Id, ct);

        _logger.LogInformation("Зарегистрирован пользователь {Email}", user.Email);
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    // ─── Вход / Выход ──────────────────────────────────────────────────

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null) =>
        View(new LoginViewModel { ReturnUrl = returnUrl });

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            isPersistent: model.RememberMe,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Неверный email или пароль.");
            return View(model);
        }

        // Merge анонимной корзины в пользовательскую (если в куке была корзина).
        var loggedUser = await _userManager.FindByEmailAsync(model.Email);
        if (loggedUser is not null)
        {
            await TryMergeAnonymousCartAsync(loggedUser.Id, ct);
        }

        _logger.LogInformation("Вход пользователя {Email}", model.Email);

        // ВАЖНО: IsLocalUrl защищает от open redirect — иначе атакующий
        // мог бы дать ссылку /Account/Login?returnUrl=https://evil.com/phishing
        // и после успешного логина юзер уехал бы на фишинговый сайт с тем же UI.
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    // ─── Профиль и смена пароля ────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        return View(new ProfileViewModel
        {
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt
        });
    }

    [HttpGet]
    public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }
            return View(model);
        }

        // RefreshSignInAsync — обновить security stamp в куке. Без этого следующий запрос
        // увидит несоответствие stamp и выкинет юзера на login. Типичная junior-ловушка.
        await _signInManager.RefreshSignInAsync(user);
        _logger.LogInformation("Пароль изменён для {Email}", user.Email);

        TempData["StatusMessage"] = "Пароль успешно изменён.";
        return RedirectToAction(nameof(Profile));
    }

    // ─── История заказов ──────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> OrderHistory(CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        var orders = await _orderService.GetForUserAsync(user.Id, ct);
        return View(new OrderHistoryViewModel
        {
            Orders = orders.Select(OrderHistoryItemViewModel.FromDto).ToList()
        });
    }

    // ─── Access denied ────────────────────────────────────────────────

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    // ─── Helpers ──────────────────────────────────────────────────────

    /// <summary>
    /// Если в куке shop_cart был Id анонимной корзины — переносим её содержимое
    /// в пользовательскую корзину и удаляем cookie. Тихо игнорируем ошибки —
    /// merge не должен ломать логин.
    /// </summary>
    private async Task TryMergeAnonymousCartAsync(Guid userId, CancellationToken ct)
    {
        if (!Request.Cookies.TryGetValue(CartContext.CookieName, out var raw)
            || !Guid.TryParse(raw, out var anonCartId))
        {
            return;
        }

        try
        {
            await _cartService.MergeAnonymousIntoUserAsync(anonCartId, userId, ct);
            _logger.LogInformation("Cart merge: anon {AnonId} → user {UserId}", anonCartId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cart merge failed for anon {AnonId} → user {UserId}", anonCartId, userId);
        }

        // Удаляем cookie независимо от результата merge — она больше не нужна.
        Response.Cookies.Delete(CartContext.CookieName);
    }
}
