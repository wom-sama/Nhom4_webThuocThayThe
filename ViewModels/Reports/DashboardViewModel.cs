using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.ViewModels.Reports;

public sealed class DashboardViewModel
{
    public IReadOnlyCollection<ReportMetricViewModel> Metrics { get; init; } = [];

    public IReadOnlyCollection<StockRiskViewModel> StockRisks { get; init; } = [];

    public IReadOnlyCollection<ExternalDataSource> ExternalSources { get; init; } = [];

    public IReadOnlyCollection<AuditLogEntry> AuditLogs { get; init; } = [];

    public required BackupSnapshotViewModel BackupSnapshot { get; init; }
}

public sealed class ReportMetricViewModel
{
    public required string Label { get; init; }

    public required string Value { get; init; }

    public required string Hint { get; init; }
}

public sealed class StockRiskViewModel
{
    public required string DrugName { get; init; }

    public required string Strength { get; init; }

    public int Quantity { get; init; }

    public required string RiskLevel { get; init; }
}

public sealed class BackupSnapshotViewModel
{
    public DateTimeOffset GeneratedAt { get; init; }

    public int DrugCount { get; init; }

    public int BatchCount { get; init; }

    public int AuditLogCount { get; init; }

    public int ExternalSourceCount { get; init; }
}
