namespace Nhom4WebThuocThayThe.Models;

public sealed class ExternalDataSource
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string SourceUrl { get; set; }

    public required string MappingStatus { get; set; }

    public DateOnly? LastSyncDate { get; set; }

    public required string Purpose { get; set; }
}
