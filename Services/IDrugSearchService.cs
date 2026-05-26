using Nhom4WebThuocThayThe.ViewModels.Search;

namespace Nhom4WebThuocThayThe.Services;

public interface IDrugSearchService
{
    DrugSearchPageViewModel Search(string? keyword, int? categoryId);

    DrugDetailViewModel? GetDetail(int id);
}
