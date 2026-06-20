using Nhom4WebThuocThayThe.Data;
using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.ViewModels.Reports;

namespace Nhom4WebThuocThayThe.Services;

public sealed class AuditLogService(PharmacyDbContext dbContext) : IAuditLogService
{
    public IReadOnlyCollection<AuditLogEntry> GetRecent(int count)
    {
        return dbContext.AuditLogs
            .AsNoTracking()
            .OrderByDescending(item => item.CreatedAt)
            .Take(count)
            .ToList();
    }

    public void Add(string actor, string action, string entity, string detail)
    {
        var nextId = dbContext.AuditLogs.Any() ? dbContext.AuditLogs.Max(item => item.Id) + 1 : 1;
        dbContext.AuditLogs.Add(new AuditLogEntry
        {
            Id = nextId,
            CreatedAt = DateTimeOffset.Now,
            Actor = actor,
            Action = action,
            Entity = entity,
            Detail = detail
        });
        dbContext.SaveChanges();
    }

    public BackupSnapshotViewModel CreateSnapshot()
    {
        return new BackupSnapshotViewModel
        {
            GeneratedAt = DateTimeOffset.Now,
            DrugCount = dbContext.Drugs.Count(),
            BatchCount = dbContext.Batches.Count(),
            AuditLogCount = dbContext.AuditLogs.Count(),
            ExternalSourceCount = dbContext.ExternalDataSources.Count()
        };
    }
}
