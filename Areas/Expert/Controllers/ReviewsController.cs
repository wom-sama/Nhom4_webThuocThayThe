using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;

namespace Nhom4WebThuocThayThe.Areas.Expert.Controllers;

[Area("Expert")]
[Authorize(Roles = AppRoles.Expert)]
public sealed class ReviewsController(IExpertReviewService expertReviewService) : Controller
{
    public IActionResult Index() => AreaView("~/Views/ExpertReviews/Index.cshtml", expertReviewService.GetPendingReviews());

    public IActionResult Evidence() => AreaView("~/Views/ExpertReviews/Evidence.cshtml", expertReviewService.GetEvidenceReviews());

    public IActionResult History() => AreaView("~/Views/ExpertReviews/History.cshtml", expertReviewService.GetDecisionHistory());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int id, string status, string note)
    {
        if (!expertReviewService.UpdateReview(id, status, User.Identity?.Name ?? "Chuyên gia", note))
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Đã cập nhật đánh giá chuyên gia.";
        return RedirectToAction(nameof(Index));
    }

    private ViewResult AreaView(string path, object model)
    {
        ViewData["Layout"] = "~/Areas/Expert/Views/Shared/_Layout.cshtml";
        return View(path, model);
    }
}
