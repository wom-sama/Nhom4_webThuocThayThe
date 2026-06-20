using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.ViewModels.Reports;

namespace Nhom4WebThuocThayThe.Services;

public sealed class AuditLogService(InMemoryPharmacyStore store) : IAuditLogService
{
    public IReadOnlyCollection<AuditLogEntry> GetRecent(int count)
    {
        return store.AuditLogs
            .OrderByDescending(item => item.CreatedAt)
            .Take(count)
            .ToList();
    }

    public void Add(string actor, string action, string entity, string detail)
    {
        store.AuditLogs.Add(new AuditLogEntry
        {
            Id = store.GetNextAuditLogId(),
            CreatedAt = DateTimeOffset.Now,
            Actor = actor,
            Action = action,
            Entity = entity,
            Detail = detail
        });
    }

    public BackupSnapshotViewModel CreateSnapshot()
    {
        return new BackupSnapshotViewModel
        {
            GeneratedAt = DateTimeOffset.Now,
            DrugCount = store.Drugs.Count,
            BatchCount = store.Batches.Count,
            AuditLogCount = store.AuditLogs.Count,
            ExternalSourceCount = store.ExternalDataSources.Count
        };
    }
}
