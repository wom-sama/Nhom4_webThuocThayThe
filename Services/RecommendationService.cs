using Nhom4WebThuocThayThe.Data;
using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.ViewModels.Search;

namespace Nhom4WebThuocThayThe.Services;

public sealed class RecommendationService(PharmacyDbContext dbContext) : IRecommendationService
{
    public IReadOnlyCollection<DrugRecommendationViewModel> GetRecommendations(int drugId, string? userEmail)
    {
        return GetRecommendationsAsync(drugId, userEmail).GetAwaiter().GetResult();
    }

    public async Task<IReadOnlyCollection<DrugRecommendationViewModel>> GetRecommendationsAsync(int drugId, string? userEmail)
    {
        var source = await dbContext.Drugs
            .AsNoTracking()
            .FirstOrDefaultAsync(drug => drug.Id == drugId && drug.IsActive);
        if (source is null)
        {
            return [];
        }

        var candidates = await dbContext.Drugs
            .AsNoTracking()
            .Where(candidate => candidate.Id != source.Id && candidate.IsActive)
            .ToListAsync();
        var drugIds = candidates.Select(item => item.Id).Append(source.Id).ToArray();
        var ingredientLinks = (await dbContext.DrugActiveIngredients
            .AsNoTracking()
            .Where(item => drugIds.Contains(item.DrugId))
            .ToListAsync())
            .GroupBy(item => item.DrugId)
            .ToDictionary(group => group.Key, group => group.First());
        ingredientLinks.TryGetValue(source.Id, out var sourceIngredient);
        var sourceIngredientId = sourceIngredient?.ActiveIngredientId;
        var sourceProfile = await GetProfileAsync(userEmail);
        var ingredientIds = ingredientLinks.Values.Select(item => item.ActiveIngredientId).Distinct().ToArray();
        var ingredients = await dbContext.ActiveIngredients
            .AsNoTracking()
            .Where(item => ingredientIds.Contains(item.Id))
            .ToDictionaryAsync(item => item.Id);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var stockByDrug = await dbContext.Batches
            .AsNoTracking()
            .Where(batch => drugIds.Contains(batch.DrugId) && batch.Quantity > 0 && batch.ExpiryDate >= today)
            .GroupBy(batch => batch.DrugId)
            .Select(group => new { DrugId = group.Key, Quantity = group.Sum(batch => batch.Quantity) })
            .ToDictionaryAsync(item => item.DrugId, item => item.Quantity);
        var dosageForms = await dbContext.DosageForms.AsNoTracking().ToDictionaryAsync(item => item.Id, item => item.Name);
        var manufacturers = await dbContext.Manufacturers.AsNoTracking().ToDictionaryAsync(item => item.Id, item => item.Name);

        return candidates
            .Select(candidate =>
            {
                ingredientLinks.TryGetValue(candidate.Id, out var candidateIngredient);
                var ingredient = candidateIngredient is not null && ingredients.TryGetValue(candidateIngredient.ActiveIngredientId, out var value)
                    ? value
                    : null;
                return BuildCandidate(
                    source,
                    candidate,
                    sourceIngredientId,
                    candidateIngredient,
                    ingredient,
                    stockByDrug.GetValueOrDefault(candidate.Id),
                    dosageForms[candidate.DosageFormId],
                    manufacturers[candidate.ManufacturerId],
                    sourceProfile);
            })
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
        DrugActiveIngredient? candidateIngredient,
        ActiveIngredient? ingredient,
        int stock,
        string dosageForm,
        string manufacturer,
        PatientSafetyProfile? profile)
    {
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
            DosageForm = dosageForm,
            Manufacturer = manufacturer,
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

    private Task<PatientSafetyProfile?> GetProfileAsync(string? userEmail)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
        {
            return Task.FromResult<PatientSafetyProfile?>(null);
        }

        return dbContext.PatientSafetyProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(profile => profile.Email == userEmail);
    }

}
