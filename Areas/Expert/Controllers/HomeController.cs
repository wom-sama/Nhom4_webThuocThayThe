using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;

namespace Nhom4WebThuocThayThe.Areas.Expert.Controllers;

[Area("Expert")]
[Authorize(Roles = AppRoles.Expert)]
public sealed class HomeController(IExpertReviewService expertReviewService) : Controller
{
    public IActionResult Index() => View(expertReviewService.GetReviews());
}
