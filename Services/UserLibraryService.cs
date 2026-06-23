using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.ViewModels.User;

namespace Nhom4WebThuocThayThe.Services;

public sealed class UserLibraryService(PharmacyDbContext dbContext) : IUserLibraryService
{
    private const int MaxHistoryItems = 50;

    public async Task<UserLibrarySummaryViewModel> GetSummaryAsync(string userEmail)
    {
        var normalizedEmail = UserAccountService.NormalizeEmail(userEmail);
        var historyCount = await dbContext.UserSearchHistories
            .AsNoTracking()
            .CountAsync(item => item.UserEmail == normalizedEmail);
        var savedCount = await dbContext.SavedDrugs
            .AsNoTracking()
            .CountAsync(item => item.UserEmail == normalizedEmail);
        var latestSearchAt = await dbContext.UserSearchHistories
            .AsNoTracking()
            .Where(item => item.UserEmail == normalizedEmail)
            .OrderByDescending(item => item.SearchedAt)
            .Select(item => (DateTimeOffset?)item.SearchedAt)
            .FirstOrDefaultAsync();

        return new UserLibrarySummaryViewModel(historyCount, savedCount, latestSearchAt);
    }

    public async Task RecordSearchAsync(string userEmail, string? keyword, int? categoryId, int resultCount)
    {
        var normalizedEmail = UserAccountService.NormalizeEmail(userEmail);
        var normalizedKeyword = keyword?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedEmail) ||
            (string.IsNullOrWhiteSpace(normalizedKeyword) && categoryId is null))
        {
            return;
        }

        string? categoryName = null;
        if (categoryId is not null)
        {
            categoryName = await dbContext.Categories
                .AsNoTracking()
                .Where(category => category.Id == categoryId.Value)
                .Select(category => category.Name)
                .FirstOrDefaultAsync();
        }

        dbContext.UserSearchHistories.Add(new UserSearchHistory
        {
            UserEmail = normalizedEmail,
            Keyword = string.IsNullOrWhiteSpace(normalizedKeyword) ? null : normalizedKeyword,
            CategoryId = categoryId,
            CategoryName = categoryName,
            ResultCount = resultCount,
            SearchedAt = DateTimeOffset.UtcNow
        });

        var staleIds = await dbContext.UserSearchHistories
            .Where(item => item.UserEmail == normalizedEmail)
            .OrderByDescending(item => item.SearchedAt)
            .Skip(MaxHistoryItems - 1)
            .Select(item => item.Id)
            .ToListAsync();
        if (staleIds.Count > 0)
        {
            await dbContext.UserSearchHistories
                .Where(item => staleIds.Contains(item.Id))
                .ExecuteDeleteAsync();
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<UserSearchHistoryItemViewModel>> GetHistoryAsync(string userEmail)
    {
        var normalizedEmail = UserAccountService.NormalizeEmail(userEmail);
        return await dbContext.UserSearchHistories
            .AsNoTracking()
            .Where(item => item.UserEmail == normalizedEmail)
            .OrderByDescending(item => item.SearchedAt)
            .Take(MaxHistoryItems)
            .Select(item => new UserSearchHistoryItemViewModel
            {
                Id = item.Id,
                Keyword = item.Keyword,
                CategoryId = item.CategoryId,
                CategoryName = item.CategoryName,
                ResultCount = item.ResultCount,
                SearchedAt = item.SearchedAt
            })
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<SavedDrugItemViewModel>> GetSavedDrugsAsync(string userEmail)
    {
        var normalizedEmail = UserAccountService.NormalizeEmail(userEmail);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var savedItems = await dbContext.SavedDrugs
            .AsNoTracking()
            .Where(item => item.UserEmail == normalizedEmail)
            .OrderByDescending(item => item.SavedAt)
            .ToListAsync();

        var drugIds = savedItems.Select(item => item.DrugId).ToArray();
        var stockByDrug = await dbContext.Batches
            .AsNoTracking()
            .Where(batch => drugIds.Contains(batch.DrugId) && batch.Quantity > 0 && batch.ExpiryDate >= today)
            .GroupBy(batch => batch.DrugId)
            .Select(group => new { DrugId = group.Key, Quantity = group.Sum(batch => batch.Quantity) })
            .ToDictionaryAsync(item => item.DrugId, item => item.Quantity);
        var drugs = await dbContext.Drugs
            .AsNoTracking()
            .Where(drug => drugIds.Contains(drug.Id))
            .ToDictionaryAsync(drug => drug.Id);
        var categories = await dbContext.Categories
            .AsNoTracking()
            .ToDictionaryAsync(category => category.Id, category => category.Name);

        return savedItems
            .Where(item => drugs.ContainsKey(item.DrugId))
            .Select(item =>
            {
                var drug = drugs[item.DrugId];
                return new SavedDrugItemViewModel
                {
                    SavedDrugId = item.Id,
                    DrugId = drug.Id,
                    Name = drug.Name,
                    Strength = drug.Strength,
                    Category = categories.GetValueOrDefault(drug.CategoryId, "Chưa phân loại"),
                    Price = drug.Price,
                    StockQuantity = stockByDrug.GetValueOrDefault(drug.Id),
                    PrescriptionRequired = drug.PrescriptionRequired,
                    SavedAt = item.SavedAt
                };
            })
            .ToList();
    }

    public async Task<bool> IsSavedAsync(string userEmail, int drugId)
    {
        var normalizedEmail = UserAccountService.NormalizeEmail(userEmail);
        return await dbContext.SavedDrugs
            .AsNoTracking()
            .AnyAsync(item => item.UserEmail == normalizedEmail && item.DrugId == drugId);
    }

    public async Task SaveDrugAsync(string userEmail, int drugId)
    {
        var normalizedEmail = UserAccountService.NormalizeEmail(userEmail);
        if (string.IsNullOrWhiteSpace(normalizedEmail) ||
            !await dbContext.Drugs.AsNoTracking().AnyAsync(drug => drug.Id == drugId) ||
            await IsSavedAsync(normalizedEmail, drugId))
        {
            return;
        }

        dbContext.SavedDrugs.Add(new SavedDrug
        {
            UserEmail = normalizedEmail,
            DrugId = drugId,
            SavedAt = DateTimeOffset.UtcNow
        });
        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveSavedDrugAsync(string userEmail, int drugId)
    {
        var normalizedEmail = UserAccountService.NormalizeEmail(userEmail);
        var saved = await dbContext.SavedDrugs
            .FirstOrDefaultAsync(item => item.UserEmail == normalizedEmail && item.DrugId == drugId);
        if (saved is null)
        {
            return;
        }

        dbContext.SavedDrugs.Remove(saved);
        await dbContext.SaveChangesAsync();
    }
}
