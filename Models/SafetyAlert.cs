namespace Nhom4WebThuocThayThe.Models;

public sealed class SafetyAlert
{
    public required string Severity { get; init; }

    public required string Title { get; init; }

    public required string Message { get; init; }
}
