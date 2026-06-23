using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;
using Nhom4WebThuocThayThe.ViewModels.ExpertReviews;

namespace Nhom4WebThuocThayThe.Areas.Expert.Controllers;

[Area("Expert")]
[Authorize(Roles = AppRoles.Expert)]
public sealed class HomeController(
    IExpertReviewService expertReviewService,
    IRoleDecisionSupportService roleDecisionSupportService) : Controller
{
    public IActionResult Index() => View(new ExpertHomeViewModel
    {
        Reviews = expertReviewService.GetReviews(),
        Insights = roleDecisionSupportService.GetInsights(AppRoles.Expert, User.FindFirstValue(ClaimTypes.Email))
    });
}
