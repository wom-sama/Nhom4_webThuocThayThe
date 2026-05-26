using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.ViewModels.Catalog;

namespace Nhom4WebThuocThayThe.Services;

public interface IDrugCatalogService
{
    IReadOnlyCollection<DrugListItemViewModel> GetDrugs();

    DrugFormViewModel CreateForm();

    DrugFormViewModel? CreateForm(int id);

    Drug? GetDrug(int id);

    void CreateDrug(DrugFormViewModel model);

    bool UpdateDrug(DrugFormViewModel model);

    IReadOnlyCollection<DrugCategory> GetCategories();
}
