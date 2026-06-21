using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.ViewModels.Inventory;

namespace Nhom4WebThuocThayThe.Services;

public sealed class InventoryService(PharmacyDbContext dbContext) : IInventoryService
{
    public int GetAvailableQuantity(int drugId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return dbContext.Batches
            .AsNoTracking()
            .Where(batch => batch.DrugId == drugId && batch.Quantity > 0 && batch.ExpiryDate >= today)
            .Sum(batch => batch.Quantity);
    }

    public Task<int> GetAvailableQuantityAsync(int drugId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return dbContext.Batches
            .AsNoTracking()
            .Where(batch => batch.DrugId == drugId && batch.Quantity > 0 && batch.ExpiryDate >= today)
            .SumAsync(batch => batch.Quantity);
    }

    public IReadOnlyCollection<StockSummaryViewModel> GetStockSummaries()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var availableQuantities = dbContext.Batches
            .AsNoTracking()
            .Where(batch => batch.Quantity > 0 && batch.ExpiryDate >= today)
            .GroupBy(batch => batch.DrugId)
            .Select(group => new
            {
                DrugId = group.Key,
                Quantity = group.Sum(batch => batch.Quantity)
            })
            .ToDictionary(item => item.DrugId, item => item.Quantity);

        return dbContext.Drugs
            .AsNoTracking()
            .OrderBy(drug => drug.Name)
            .AsEnumerable()
            .Select(drug => new StockSummaryViewModel
            {
                DrugId = drug.Id,
                DrugName = drug.Name,
                Strength = drug.Strength,
                Quantity = availableQuantities.GetValueOrDefault(drug.Id)
            })
            .ToList();
    }

    public IReadOnlyCollection<DrugBatch> GetBatches()
    {
        return dbContext.Batches
            .AsNoTracking()
            .OrderBy(batch => batch.ExpiryDate)
            .ThenBy(batch => batch.BatchNumber)
            .ToList();
    }

    public IReadOnlyCollection<BatchListItemViewModel> GetBatchList()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var nearExpiryLimit = today.AddDays(90);

        var drugs = dbContext.Drugs.AsNoTracking().ToDictionary(item => item.Id);
        var warehouses = dbContext.Warehouses.AsNoTracking().ToDictionary(item => item.Id);

        return dbContext.Batches
            .AsNoTracking()
            .OrderBy(batch => batch.ExpiryDate)
            .ThenBy(batch => batch.BatchNumber)
            .AsEnumerable()
            .Select(batch =>
            {
                var drug = drugs[batch.DrugId];
                var warehouse = warehouses[batch.WarehouseId];

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
        var nextId = dbContext.Batches.Any() ? dbContext.Batches.Max(item => item.Id) + 1 : 1;
        dbContext.Batches.Add(new DrugBatch
        {
            Id = nextId,
            DrugId = model.DrugId,
            WarehouseId = model.WarehouseId,
            BatchNumber = model.BatchNumber.Trim(),
            Quantity = model.Quantity,
            ImportedDate = model.ImportedDate,
            ExpiryDate = model.ExpiryDate
        });
        dbContext.SaveChanges();
    }

    private BatchFormViewModel Populate(BatchFormViewModel model)
    {
        model.Drugs = dbContext.Drugs
            .AsNoTracking()
            .OrderBy(drug => drug.Name)
            .Select(drug => new SelectListItem($"{drug.Name} - {drug.Strength}", drug.Id.ToString(), drug.Id == model.DrugId))
            .ToList();

        model.Warehouses = dbContext.Warehouses
            .AsNoTracking()
            .OrderBy(warehouse => warehouse.Name)
            .Select(warehouse => new SelectListItem(warehouse.Name, warehouse.Id.ToString(), warehouse.Id == model.WarehouseId))
            .ToList();

        return model;
    }
}
