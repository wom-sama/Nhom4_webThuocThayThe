namespace Nhom4WebThuocThayThe.ViewModels.ExpertReviews;

public sealed class ExpertReviewListItemViewModel
{
    public int Id { get; init; }

    public required string SourceDrug { get; init; }

    public int SourceDrugId { get; init; }

    public required string RecommendedDrug { get; init; }

    public int RecommendedDrugId { get; init; }

    public int Score { get; init; }

    public required string ScoreLabel { get; init; }

    public required string Status { get; init; }

    public required string StatusLabel { get; init; }

    public required string Reviewer { get; init; }

    public required string Note { get; init; }

    public DateTimeOffset UpdatedAt { get; init; }

    public required string StockSignal { get; init; }

    public required string SafetySignal { get; init; }

    public required string EvidenceLevel { get; init; }

    public IReadOnlyCollection<string> RuleSignals { get; init; } = [];
}
