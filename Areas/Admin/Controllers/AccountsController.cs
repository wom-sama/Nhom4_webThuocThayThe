using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;
using Nhom4WebThuocThayThe.ViewModels.Admin;

namespace Nhom4WebThuocThayThe.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = AppRoles.Admin)]
public sealed class AccountsController(IUserAccountService userAccountService) : Controller
{
    public IActionResult Index() => View(CreatePageModel(new AccountCreateViewModel()));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind(Prefix = "Create")] AccountCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(Index), CreatePageModel(model));
        }

        var result = userAccountService.CreateAccount(
            model.DisplayName,
            model.Email,
            model.Role,
            model.Password,
            CurrentActor());
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(nameof(Index), CreatePageModel(model));
        }

        TempData["StatusMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetLocked(string email, bool isLocked)
    {
        var result = userAccountService.SetLocked(email, isLocked, CurrentActor());
        TempData[result.IsSuccess ? "StatusMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ResetPassword(PasswordResetViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Mật khẩu mới cần tối thiểu 8 ký tự.";
            return RedirectToAction(nameof(Index));
        }

        var result = userAccountService.ResetPassword(model.Email, model.NewPassword, CurrentActor());
        TempData[result.IsSuccess ? "StatusMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    private AccountManagementViewModel CreatePageModel(AccountCreateViewModel createModel)
    {
        return new AccountManagementViewModel
        {
            Create = createModel,
            RoleOptions = AppRoles.All.Select(role => new SelectListItem(role, role, role == createModel.Role)),
            Accounts = userAccountService.GetAccounts()
                .Select(account => new AccountListItemViewModel
                {
                    Email = account.Email,
                    DisplayName = account.DisplayName,
                    Role = account.Role,
                    Source = account.Source,
                    IsLocked = account.IsLocked,
                    CanManage = account.CanManage,
                    CreatedAt = account.CreatedAt,
                    UpdatedAt = account.UpdatedAt
                })
                .ToList()
        };
    }

    private string CurrentActor() => User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "admin";
}
