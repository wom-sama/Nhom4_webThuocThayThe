namespace Nhom4WebThuocThayThe.Models;

public sealed class AuditLogEntry
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public required string Actor { get; set; }

    public required string Action { get; set; }

    public required string Entity { get; set; }

    public required string Detail { get; set; }
}
