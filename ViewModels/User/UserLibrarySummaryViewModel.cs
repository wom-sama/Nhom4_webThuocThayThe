namespace Nhom4WebThuocThayThe.ViewModels.User;

public sealed record UserLibrarySummaryViewModel(
    int SearchCount,
    int SavedDrugCount,
    DateTimeOffset? LatestSearchAt);
