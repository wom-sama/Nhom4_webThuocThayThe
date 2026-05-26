using Microsoft.AspNetCore.Mvc.Rendering;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.ViewModels.Inventory;

namespace Nhom4WebThuocThayThe.Services;

public sealed class InventoryService(InMemoryPharmacyStore store) : IInventoryService
{
    public int GetAvailableQuantity(int drugId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return store.Batches
            .Where(batch => batch.DrugId == drugId && batch.IsUsable(today))
            .Sum(batch => batch.Quantity);
    }

    public IReadOnlyCollection<StockSummaryViewModel> GetStockSummaries()
    {
        return store.Drugs
            .OrderBy(drug => drug.Name)
            .Select(drug => new StockSummaryViewModel
            {
                DrugId = drug.Id,
                DrugName = drug.Name,
                Strength = drug.Strength,
                Quantity = GetAvailableQuantity(drug.Id)
            })
            .ToList();
    }

    public IReadOnlyCollection<DrugBatch> GetBatches()
    {
        return store.Batches
            .OrderBy(batch => batch.ExpiryDate)
            .ThenBy(batch => batch.BatchNumber)
            .ToList();
    }

    public IReadOnlyCollection<BatchListItemViewModel> GetBatchList()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var nearExpiryLimit = today.AddDays(90);

        return store.Batches
            .OrderBy(batch => batch.ExpiryDate)
            .ThenBy(batch => batch.BatchNumber)
            .Select(batch =>
            {
                var drug = store.Drugs.First(item => item.Id == batch.DrugId);
                var warehouse = store.Warehouses.First(item => item.Id == batch.WarehouseId);

                return new BatchListItemViewModel
                {
                    Id = batch.Id,
                    DrugName = drug.Name,
                    Strength = drug.Strength,
                    WarehouseName = warehouse.Name,
                    BatchNumber = batch.BatchNumber,
                    Quantity = batch.Quantity,
                    ImportedDate = batch.ImportedDate,
                    ExpiryDate = batch.ExpiryDate,
                    IsExpired = batch.ExpiryDate < today,
                    IsNearExpiry = batch.ExpiryDate >= today && batch.ExpiryDate <= nearExpiryLimit,
                    IsUsable = batch.IsUsable(today)
                };
            })
            .ToList();
    }

    public BatchFormViewModel CreateBatchForm()
    {
        return Populate(new BatchFormViewModel());
    }

    public BatchFormViewModel CreateBatchForm(BatchFormViewModel model)
    {
        return Populate(model);
    }

    public void AddBatch(BatchFormViewModel model)
    {
        store.Batches.Add(new DrugBatch
        {
            Id = store.GetNextBatchId(),
            DrugId = model.DrugId,
            WarehouseId = model.WarehouseId,
            BatchNumber = model.BatchNumber.Trim(),
            Quantity = model.Quantity,
            ImportedDate = model.ImportedDate,
            ExpiryDate = model.ExpiryDate
        });
    }

    private BatchFormViewModel Populate(BatchFormViewModel model)
    {
        model.Drugs = store.Drugs
            .OrderBy(drug => drug.Name)
            .Select(drug => new SelectListItem($"{drug.Name} - {drug.Strength}", drug.Id.ToString(), drug.Id == model.DrugId))
            .ToList();

        model.Warehouses = store.Warehouses
            .OrderBy(warehouse => warehouse.Name)
            .Select(warehouse => new SelectListItem(warehouse.Name, warehouse.Id.ToString(), warehouse.Id == model.WarehouseId))
            .ToList();

        return model;
    }
}
