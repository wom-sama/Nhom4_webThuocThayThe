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
        var reasons = new List<string>();
        var alerts = new List<SafetyAlert>();
        var score = 0;

        if (sourceIngredientId is not null && candidateIngredient?.ActiveIngredientId == sourceIngredientId)
        {
            score += 45;
            reasons.Add("Cung hoat chat voi thuoc chinh.");
        }
        else if (candidate.CategoryId == source.CategoryId)
        {
            score += 20;
            reasons.Add("Cung nhom dieu tri, can duoc si xac nhan truoc khi thay the.");
        }

        if (string.Equals(candidate.Strength, source.Strength, StringComparison.OrdinalIgnoreCase))
        {
            score += 20;
            reasons.Add("Ham luong trung khop.");
        }

        if (candidate.DosageFormId == source.DosageFormId)
        {
            score += 15;
            reasons.Add("Dang bao che tuong thich.");
        }

        if (stock > 0)
        {
            score += 15;
            reasons.Add("Con hang trong kho kha dung.");
        }
        else
        {
            alerts.Add(new SafetyAlert
            {
                Severity = "High",
                Title = "Het hang",
                Message = "Ung vien thay the hien khong co lo kha dung."
            });
        }

        if (candidate.Price <= source.Price)
        {
            score += 5;
            reasons.Add("Gia khong cao hon thuoc chinh.");
        }

        if (candidate.PrescriptionRequired)
        {
            score -= 10;
            alerts.Add(new SafetyAlert
            {
                Severity = "Medium",
                Title = "Can ke don",
                Message = "Thuoc can duoc bac si hoac duoc si xac nhan truoc khi tu van."
            });
        }

        if (!string.IsNullOrWhiteSpace(ingredient?.Warning))
        {
            alerts.Add(new SafetyAlert
            {
                Severity = candidate.PrescriptionRequired ? "Medium" : "Low",
                Title = "Canh bao hoat chat",
                Message = ingredient.Warning
            });
        }

        if (candidateIngredient is not null &&
            profile?.AllergyActiveIngredientIds.Contains(candidateIngredient.ActiveIngredientId) == true)
        {
            score -= 25;
            alerts.Add(new SafetyAlert
            {
                Severity = "High",
                Title = "Di ung hoat chat",
                Message = $"{profile.DisplayName} co ho so di ung voi hoat chat {ingredient?.Name ?? "nay"}."
            });
        }

        if (!string.IsNullOrWhiteSpace(candidate.Contraindications) &&
            candidate.Contraindications.Contains("di ung", StringComparison.OrdinalIgnoreCase))
        {
            alerts.Add(new SafetyAlert
            {
                Severity = "Medium",
                Title = "Chong chi dinh",
                Message = candidate.Contraindications
            });
        }

        score = Math.Clamp(score, 0, 100);

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
            Score = score,
            ScoreLabel = GetScoreLabel(score),
            Reasons = reasons,
            Alerts = alerts
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

    private static string GetScoreLabel(int score)
    {
        return score switch
        {
            >= 85 => "Rat phu hop",
            >= 70 => "Phu hop",
            >= 55 => "Can xem xet",
            _ => "Chi tham khao"
        };
    }
}
