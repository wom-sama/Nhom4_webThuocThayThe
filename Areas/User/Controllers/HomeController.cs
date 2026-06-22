using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = AppRoles.User)]
public sealed class HomeController : Controller
{
    public IActionResult Index() => View();

    public IActionResult History() => View();

    public IActionResult Saved() => View();
}
