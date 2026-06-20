using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.ViewModels.Reports;

public sealed class DatabaseBackupViewModel
{
    public required string Version { get; init; }

    public DateTimeOffset GeneratedAt { get; init; }

    public int DrugCount => Drugs.Count;

    public IReadOnlyCollection<DrugCategory> Categories { get; init; } = [];

    public IReadOnlyCollection<DosageForm> DosageForms { get; init; } = [];

    public IReadOnlyCollection<MeasurementUnit> Units { get; init; } = [];

    public IReadOnlyCollection<Manufacturer> Manufacturers { get; init; } = [];

    public IReadOnlyCollection<ActiveIngredient> ActiveIngredients { get; init; } = [];

    public IReadOnlyCollection<Drug> Drugs { get; init; } = [];

    public IReadOnlyCollection<DrugActiveIngredient> DrugActiveIngredients { get; init; } = [];

    public IReadOnlyCollection<Warehouse> Warehouses { get; init; } = [];

    public IReadOnlyCollection<DrugBatch> Batches { get; init; } = [];

    public IReadOnlyCollection<PatientSafetyProfile> PatientSafetyProfiles { get; init; } = [];

    public IReadOnlyCollection<ExternalDataSource> ExternalDataSources { get; init; } = [];

    public IReadOnlyCollection<AuditLogEntry> AuditLogs { get; init; } = [];

    public IReadOnlyCollection<ExpertReviewItem> ExpertReviews { get; init; } = [];
}
