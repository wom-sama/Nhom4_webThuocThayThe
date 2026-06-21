using Nhom4WebThuocThayThe.ViewModels.Search;

namespace Nhom4WebThuocThayThe.Services;

public interface IDrugSearchService
{
    Task<DrugSearchPageViewModel> SearchAsync(string? keyword, int? categoryId);

    Task<DrugDetailViewModel?> GetDetailAsync(int id, string? userEmail);
}
