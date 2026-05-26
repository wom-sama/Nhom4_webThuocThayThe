using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.ViewModels.Inventory;

namespace Nhom4WebThuocThayThe.Services;

public interface IInventoryService
{
    int GetAvailableQuantity(int drugId);

    IReadOnlyCollection<StockSummaryViewModel> GetStockSummaries();

    IReadOnlyCollection<DrugBatch> GetBatches();

    BatchFormViewModel CreateBatchForm();

    void AddBatch(BatchFormViewModel model);
}
