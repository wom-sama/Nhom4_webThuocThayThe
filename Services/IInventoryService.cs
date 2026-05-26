using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.ViewModels.Inventory;

namespace Nhom4WebThuocThayThe.Services;

public interface IInventoryService
{
    int GetAvailableQuantity(int drugId);

    IReadOnlyCollection<StockSummaryViewModel> GetStockSummaries();

    IReadOnlyCollection<DrugBatch> GetBatches();

    IReadOnlyCollection<BatchListItemViewModel> GetBatchList();

    BatchFormViewModel CreateBatchForm();

    BatchFormViewModel CreateBatchForm(BatchFormViewModel model);

    void AddBatch(BatchFormViewModel model);
}
