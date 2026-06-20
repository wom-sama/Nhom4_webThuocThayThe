namespace Nhom4WebThuocThayThe.ViewModels.ExpertReviews;

public sealed class ExpertReviewListItemViewModel
{
    public int Id { get; init; }

    public required string SourceDrug { get; init; }

    public required string RecommendedDrug { get; init; }

    public int Score { get; init; }

    public required string Status { get; init; }

    public required string Reviewer { get; init; }

    public required string Note { get; init; }

    public DateTimeOffset UpdatedAt { get; init; }
}
