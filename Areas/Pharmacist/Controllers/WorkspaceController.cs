using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;
using Nhom4WebThuocThayThe.Services.Ai;

namespace Nhom4WebThuocThayThe.Areas.Pharmacist.Controllers;

[Area("Pharmacist")]
[Authorize(Roles = AppRoles.Pharmacist)]
public sealed class WorkspaceController(
    IDrugSearchService drugSearchService,
    IAiRecommendationExplanationService aiExplanationService) : Controller
{
    public async Task<IActionResult> Index(string? keyword, int? categoryId)
    {
        return AreaView("~/Views/Drugs/Index.cshtml", await drugSearchService.SearchAsync(keyword, categoryId));
    }

    public async Task<IActionResult> Details(int id)
    {
        var model = await drugSearchService.GetDetailAsync(id, User.FindFirstValue(ClaimTypes.Email));
        return model is null ? NotFound() : AreaView("~/Views/Drugs/Details.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("ai")]
    public async Task<IActionResult> ExplainAlternative(int sourceId, int candidateId, CancellationToken cancellationToken)
    {
        var source = await drugSearchService.GetDetailAsync(sourceId, null);
        var candidate = source?.Alternatives.FirstOrDefault(item => item.Id == candidateId);
        if (source is null || candidate is null)
        {
            return NotFound();
        }

        var context = new AiRecommendationContext(
            source.Name,
            source.Strength,
            candidate.Name,
            candidate.Strength,
            candidate.ActiveIngredient,
            candidate.DosageForm,
            candidate.PrescriptionRequired,
            candidate.StockQuantity,
            candidate.Score,
            candidate.Reasons,
            candidate.Alerts.Select(alert => $"{alert.Title}: {alert.Message}").ToArray());
        var explanation = await aiExplanationService.ExplainAsync(context, cancellationToken);

        return Json(new
        {
            explanation.Summary,
            explanation.Checkpoints,
            explanation.Limitations,
            explanation.IsAiGenerated,
            explanation.Provider,
            explanation.Model
        });
    }

    private ViewResult AreaView(string path, object model)
    {
        ViewData["Layout"] = "~/Areas/Pharmacist/Views/Shared/_Layout.cshtml";
        return View(path, model);
    }
}
