using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom4WebThuocThayThe.Services;

namespace Nhom4WebThuocThayThe.Controllers;

[Authorize(Policy = "ExpertReviewer")]
public sealed class ExpertReviewsController(IExpertReviewService expertReviewService) : Controller
{
    public IActionResult Index()
    {
        return View(expertReviewService.GetReviews());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int id, string status, string note)
    {
        if (!expertReviewService.UpdateReview(
                id,
                status,
                User.Identity?.Name ?? "Chuyen gia",
                note))
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Da cap nhat danh gia chuyen gia.";
        return RedirectToAction(nameof(Index));
    }
}
