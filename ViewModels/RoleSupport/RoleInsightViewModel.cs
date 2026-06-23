namespace Nhom4WebThuocThayThe.ViewModels.RoleSupport;

public sealed class RoleInsightViewModel
{
    public required string Title { get; init; }

    public required string Summary { get; init; }

    public required string Severity { get; init; }

    public required string ActionLabel { get; init; }

    public required string ActionUrl { get; init; }

    public IReadOnlyCollection<string> Signals { get; init; } = [];
}
