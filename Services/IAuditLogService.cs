using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.ViewModels.Reports;

namespace Nhom4WebThuocThayThe.Services;

public interface IAuditLogService
{
    IReadOnlyCollection<AuditLogEntry> GetRecent(int count);

    void Add(string actor, string action, string entity, string detail);

    BackupSnapshotViewModel CreateSnapshot();
}
