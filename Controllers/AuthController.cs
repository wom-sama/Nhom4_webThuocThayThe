using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;
using Nhom4WebThuocThayThe.ViewModels.Auth;

namespace Nhom4WebThuocThayThe.Controllers;

public sealed class AuthController(IUserAccountService userAccountService) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToRole(User.FindFirstValue(ClaimTypes.Role), returnUrl);
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = userAccountService.ValidateCredentials(model.Email, model.Password);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Email),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = false });

        return RedirectToRole(user.Role, model.ReturnUrl);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToRole(string? role, string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) &&
            Url.IsLocalUrl(returnUrl) &&
            IsReturnUrlAllowed(role, returnUrl))
        {
            return Redirect(returnUrl);
        }

        return role switch
        {
            AppRoles.Admin => RedirectToAction("Index", "Home", new { area = "Admin" }),
            AppRoles.Pharmacist => RedirectToAction("Index", "Home", new { area = "Pharmacist" }),
            AppRoles.Expert => RedirectToAction("Index", "Home", new { area = "Expert" }),
            AppRoles.User => RedirectToAction("Index", "Home", new { area = "User" }),
            _ => RedirectToAction("Index", "Home", new { area = string.Empty })
        };
    }

    private static bool IsReturnUrlAllowed(string? role, string returnUrl)
    {
        var path = returnUrl.Split('?', '#')[0];
        if (path.StartsWith("/Drugs", StringComparison.OrdinalIgnoreCase) ||
            path.Equals("/", StringComparison.Ordinal))
        {
            return true;
        }

        var areaPrefix = role switch
        {
            AppRoles.Admin => "/Admin",
            AppRoles.Pharmacist => "/Pharmacist",
            AppRoles.Expert => "/Expert",
            AppRoles.User => "/User",
            _ => null
        };

        return areaPrefix is not null && path.StartsWith(areaPrefix, StringComparison.OrdinalIgnoreCase);
    }
}
