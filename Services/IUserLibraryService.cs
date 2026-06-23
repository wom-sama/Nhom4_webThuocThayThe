using Nhom4WebThuocThayThe.ViewModels.User;

namespace Nhom4WebThuocThayThe.Services;

public interface IUserLibraryService
{
    Task<UserLibrarySummaryViewModel> GetSummaryAsync(string userEmail);

    Task RecordSearchAsync(string userEmail, string? keyword, int? categoryId, int resultCount);

    Task<IReadOnlyCollection<UserSearchHistoryItemViewModel>> GetHistoryAsync(string userEmail);

    Task<IReadOnlyCollection<SavedDrugItemViewModel>> GetSavedDrugsAsync(string userEmail);

    Task<bool> IsSavedAsync(string userEmail, int drugId);

    Task SaveDrugAsync(string userEmail, int drugId);

    Task RemoveSavedDrugAsync(string userEmail, int drugId);
}
