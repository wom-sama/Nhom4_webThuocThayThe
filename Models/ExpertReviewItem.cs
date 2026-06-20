namespace Nhom4WebThuocThayThe.Models;

public sealed class ExpertReviewItem
{
    public int Id { get; init; }

    public int SourceDrugId { get; init; }

    public int RecommendedDrugId { get; init; }

    public int Score { get; init; }

    public required string Status { get; set; }

    public required string Reviewer { get; set; }

    public required string Note { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
