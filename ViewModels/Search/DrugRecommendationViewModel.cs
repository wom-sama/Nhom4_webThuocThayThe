using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.ViewModels.Search;

public sealed class DrugRecommendationViewModel
{
    public int Id { get; init; }

    public required string Name { get; init; }

    public required string Strength { get; init; }

    public required string DosageForm { get; init; }

    public required string Manufacturer { get; init; }

    public required string ActiveIngredient { get; init; }

    public decimal Price { get; init; }

    public int StockQuantity { get; init; }

    public bool PrescriptionRequired { get; init; }

    public int Score { get; init; }

    public required string ScoreLabel { get; init; }

    public IReadOnlyCollection<string> Reasons { get; init; } = [];

    public IReadOnlyCollection<SafetyAlert> Alerts { get; init; } = [];

    public bool HasHighRiskAlert => Alerts.Any(alert => alert.Severity == "High");
}
