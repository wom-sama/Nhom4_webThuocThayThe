using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.Services;

internal sealed record RecommendationEvaluation(
    int Score,
    IReadOnlyCollection<string> Reasons,
    IReadOnlyCollection<SafetyAlert> Alerts);

internal static class RecommendationScoring
{
    public static RecommendationEvaluation Evaluate(
        Drug source,
        Drug candidate,
        int? sourceIngredientId,
        int? candidateIngredientId,
        int stock,
        bool patientIsAllergic,
        string? ingredientName,
        string? ingredientWarning,
        string? patientDisplayName)
    {
        var reasons = new List<string>();
        var alerts = new List<SafetyAlert>();
        var score = 0;

        if (sourceIngredientId is not null && candidateIngredientId == sourceIngredientId)
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

        if (!string.IsNullOrWhiteSpace(ingredientWarning))
        {
            alerts.Add(new SafetyAlert
            {
                Severity = candidate.PrescriptionRequired ? "Medium" : "Low",
                Title = "Canh bao hoat chat",
                Message = ingredientWarning
            });
        }

        if (patientIsAllergic)
        {
            score -= 25;
            alerts.Add(new SafetyAlert
            {
                Severity = "High",
                Title = "Di ung hoat chat",
                Message = $"{patientDisplayName ?? "Nguoi dung"} co ho so di ung voi hoat chat {ingredientName ?? "nay"}."
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

        return new RecommendationEvaluation(Math.Clamp(score, 0, 100), reasons, alerts);
    }

    public static string GetScoreLabel(int score)
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
