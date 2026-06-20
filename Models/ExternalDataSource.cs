namespace Nhom4WebThuocThayThe.Models;

public sealed class ExternalDataSource
{
    public int Id { get; init; }

    public required string Name { get; init; }

    public required string SourceUrl { get; init; }

    public required string MappingStatus { get; set; }

    public DateOnly? LastSyncDate { get; set; }

    public required string Purpose { get; init; }
}
