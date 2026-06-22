using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;

namespace Nhom4WebThuocThayThe.UnitTests;

public sealed class RecommendationScoringTests
{
    [Fact]
    public void ExactAvailableAlternative_ReachesMaximumScore()
    {
        var result = Evaluate(candidateIngredientId: 10, stock: 20);

        Assert.Equal(100, result.Score);
        Assert.Empty(result.Alerts);
        Assert.Contains(result.Reasons, reason => reason.Contains("Cùng hoạt chất"));
    }

    [Fact]
    public void PrescriptionAlternative_IsPenalizedAndWarned()
    {
        var result = Evaluate(candidateIngredientId: 10, stock: 20, prescriptionRequired: true);

        Assert.Equal(90, result.Score);
        Assert.Contains(result.Alerts, alert => alert.Title == "Cần kê đơn" && alert.Severity == "Medium");
    }

    [Fact]
    public void PatientAllergy_AddsHighRiskAlertAndPenalty()
    {
        var result = Evaluate(candidateIngredientId: 10, stock: 20, patientIsAllergic: true);

        Assert.Equal(75, result.Score);
        Assert.Contains(result.Alerts, alert => alert.Title == "Dị ứng hoạt chất" && alert.Severity == "High");
    }

    [Fact]
    public void OutOfStockAlternative_AddsHighRiskAlert()
    {
        var result = Evaluate(candidateIngredientId: 10, stock: 0);

        Assert.Equal(85, result.Score);
        Assert.Contains(result.Alerts, alert => alert.Title == "Hết hàng" && alert.Severity == "High");
    }

    [Theory]
    [InlineData(90, "Rất phù hợp")]
    [InlineData(70, "Phù hợp")]
    [InlineData(55, "Cần xem xét")]
    [InlineData(54, "Chỉ tham khảo")]
    public void ScoreLabels_RespectBoundaries(int score, string expected)
    {
        Assert.Equal(expected, RecommendationScoring.GetScoreLabel(score));
    }

    private static RecommendationEvaluation Evaluate(
        int? candidateIngredientId,
        int stock,
        bool prescriptionRequired = false,
        bool patientIsAllergic = false)
    {
        var source = Drug(1, "Thuoc goc", price: 100, prescriptionRequired: false);
        var candidate = Drug(2, "Thuoc thay the", price: 90, prescriptionRequired);
        return RecommendationScoring.Evaluate(
            source,
            candidate,
            sourceIngredientId: 10,
            candidateIngredientId,
            stock,
            patientIsAllergic,
            ingredientName: "Paracetamol",
            ingredientWarning: null,
            patientDisplayName: "Nguoi benh");
    }

    private static Drug Drug(int id, string name, decimal price, bool prescriptionRequired)
    {
        return new Drug
        {
            Id = id,
            Name = name,
            Strength = "500 mg",
            Price = price,
            CategoryId = 1,
            DosageFormId = 1,
            UnitId = 1,
            ManufacturerId = 1,
            PrescriptionRequired = prescriptionRequired,
            IsActive = true
        };
    }
}
