namespace Nhom4WebThuocThayThe.Models;

public sealed class ExpertReviewItem
{
    public int Id { get; set; }

    public int SourceDrugId { get; set; }

    public int RecommendedDrugId { get; set; }

    public int Score { get; set; }

    public required string Status { get; set; }

    public required string Reviewer { get; set; }

    public required string Note { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
