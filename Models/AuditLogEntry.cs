namespace Nhom4WebThuocThayThe.Models;

public sealed class AuditLogEntry
{
    public int Id { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public required string Actor { get; init; }

    public required string Action { get; init; }

    public required string Entity { get; init; }

    public required string Detail { get; init; }
}
