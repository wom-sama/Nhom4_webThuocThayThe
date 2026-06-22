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
            reasons.Add("Cùng hoạt chất với thuốc chính.");
        }
        else if (candidate.CategoryId == source.CategoryId)
        {
            score += 20;
            reasons.Add("Cùng nhóm điều trị, cần dược sĩ xác nhận trước khi thay thế.");
        }

        if (string.Equals(candidate.Strength, source.Strength, StringComparison.OrdinalIgnoreCase))
        {
            score += 20;
            reasons.Add("Hàm lượng trùng khớp.");
        }

        if (candidate.DosageFormId == source.DosageFormId)
        {
            score += 15;
            reasons.Add("Dạng bào chế tương thích.");
        }

        if (stock > 0)
        {
            score += 15;
            reasons.Add("Còn hàng trong kho khả dụng.");
        }
        else
        {
            alerts.Add(new SafetyAlert
            {
                Severity = "High",
                Title = "Hết hàng",
                Message = "Ứng viên thay thế hiện không có lô khả dụng."
            });
        }

        if (candidate.Price <= source.Price)
        {
            score += 5;
            reasons.Add("Giá không cao hơn thuốc chính.");
        }

        if (candidate.PrescriptionRequired)
        {
            score -= 10;
            alerts.Add(new SafetyAlert
            {
                Severity = "Medium",
                Title = "Cần kê đơn",
                Message = "Thuốc cần được bác sĩ hoặc dược sĩ xác nhận trước khi tư vấn."
            });
        }

        if (!string.IsNullOrWhiteSpace(ingredientWarning))
        {
            alerts.Add(new SafetyAlert
            {
                Severity = candidate.PrescriptionRequired ? "Medium" : "Low",
                Title = "Cảnh báo hoạt chất",
                Message = ingredientWarning
            });
        }

        if (patientIsAllergic)
        {
            score -= 25;
            alerts.Add(new SafetyAlert
            {
                Severity = "High",
                Title = "Dị ứng hoạt chất",
                Message = $"{patientDisplayName ?? "Người dùng"} có hồ sơ dị ứng với hoạt chất {ingredientName ?? "này"}."
            });
        }

        if (!string.IsNullOrWhiteSpace(candidate.Contraindications) &&
            (candidate.Contraindications.Contains("di ung", StringComparison.OrdinalIgnoreCase) ||
             candidate.Contraindications.Contains("dị ứng", StringComparison.OrdinalIgnoreCase)))
        {
            alerts.Add(new SafetyAlert
            {
                Severity = "Medium",
                Title = "Chống chỉ định",
                Message = candidate.Contraindications
            });
        }

        return new RecommendationEvaluation(Math.Clamp(score, 0, 100), reasons, alerts);
    }

    public static string GetScoreLabel(int score)
    {
        return score switch
        {
            >= 85 => "Rất phù hợp",
            >= 70 => "Phù hợp",
            >= 55 => "Cần xem xét",
            _ => "Chỉ tham khảo"
        };
    }
}
