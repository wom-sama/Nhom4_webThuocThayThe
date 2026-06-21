using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using Nhom4WebThuocThayThe.Services;
using Nhom4WebThuocThayThe.Services.Ai;

namespace Nhom4WebThuocThayThe.Controllers;

public sealed class DrugsController(
    IDrugSearchService drugSearchService,
    IAiRecommendationExplanationService aiExplanationService) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index(string? keyword, int? categoryId)
    {
        return View(await drugSearchService.SearchAsync(keyword, categoryId));
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var model = await drugSearchService.GetDetailAsync(id, userEmail);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("ai")]
    public async Task<IActionResult> ExplainAlternative(
        int sourceId,
        int candidateId,
        CancellationToken cancellationToken)
    {
        // AI receives a generic lookup without the signed-in user's safety profile.
        var source = await drugSearchService.GetDetailAsync(sourceId, null);
        if (source is null)
        {
            return NotFound();
        }

        var candidate = source.Alternatives.FirstOrDefault(item => item.Id == candidateId);
        if (candidate is null)
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
}
