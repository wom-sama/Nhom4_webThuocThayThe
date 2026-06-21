using Nhom4WebThuocThayThe.Data;
using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.ViewModels.Search;

namespace Nhom4WebThuocThayThe.Services;

public sealed class RecommendationService(
    PharmacyDbContext dbContext,
    IInventoryService inventoryService) : IRecommendationService
{
    public IReadOnlyCollection<DrugRecommendationViewModel> GetRecommendations(int drugId, string? userEmail)
    {
        var source = dbContext.Drugs
            .AsNoTracking()
            .FirstOrDefault(drug => drug.Id == drugId && drug.IsActive);
        if (source is null)
        {
            return [];
        }

        var sourceIngredient = GetPrimaryIngredient(source.Id);
        var sourceIngredientId = sourceIngredient?.ActiveIngredientId;
        var sourceProfile = GetProfile(userEmail);

        return dbContext.Drugs
            .AsNoTracking()
            .Where(candidate => candidate.Id != source.Id && candidate.IsActive)
            .AsEnumerable()
            .Select(candidate => BuildCandidate(source, candidate, sourceIngredientId, sourceProfile))
            .Where(candidate => candidate.Score >= 45 || candidate.Reasons.Any(reason => reason.Contains("cung hoat chat", StringComparison.OrdinalIgnoreCase)))
            .OrderByDescending(candidate => candidate.StockQuantity > 0)
            .ThenByDescending(candidate => candidate.Score)
            .ThenBy(candidate => candidate.HasHighRiskAlert)
            .ThenBy(candidate => candidate.Price)
            .ToList();
    }

    private DrugRecommendationViewModel BuildCandidate(
        Drug source,
        Drug candidate,
        int? sourceIngredientId,
        PatientSafetyProfile? profile)
    {
        var candidateIngredient = GetPrimaryIngredient(candidate.Id);
        var ingredient = candidateIngredient is null
            ? null
            : dbContext.ActiveIngredients.AsNoTracking().FirstOrDefault(item => item.Id == candidateIngredient.ActiveIngredientId);
        var stock = inventoryService.GetAvailableQuantity(candidate.Id);
        var patientIsAllergic = candidateIngredient is not null &&
            profile?.AllergyActiveIngredientIds.Contains(candidateIngredient.ActiveIngredientId) == true;
        var evaluation = RecommendationScoring.Evaluate(
            source,
            candidate,
            sourceIngredientId,
            candidateIngredient?.ActiveIngredientId,
            stock,
            patientIsAllergic,
            ingredient?.Name,
            ingredient?.Warning,
            profile?.DisplayName);

        return new DrugRecommendationViewModel
        {
            Id = candidate.Id,
            Name = candidate.Name,
            Strength = candidate.Strength,
            DosageForm = dbContext.DosageForms.AsNoTracking().First(form => form.Id == candidate.DosageFormId).Name,
            Manufacturer = dbContext.Manufacturers.AsNoTracking().First(manufacturer => manufacturer.Id == candidate.ManufacturerId).Name,
            ActiveIngredient = ingredient?.Name ?? "Chua khai bao",
            Price = candidate.Price,
            StockQuantity = stock,
            PrescriptionRequired = candidate.PrescriptionRequired,
            Score = evaluation.Score,
            ScoreLabel = RecommendationScoring.GetScoreLabel(evaluation.Score),
            Reasons = evaluation.Reasons,
            Alerts = evaluation.Alerts
        };
    }

    private DrugActiveIngredient? GetPrimaryIngredient(int drugId)
    {
        return dbContext.DrugActiveIngredients
            .AsNoTracking()
            .FirstOrDefault(item => item.DrugId == drugId);
    }

    private PatientSafetyProfile? GetProfile(string? userEmail)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
        {
            return null;
        }

        return dbContext.PatientSafetyProfiles
            .AsNoTracking()
            .FirstOrDefault(profile => profile.Email == userEmail);
    }

}
