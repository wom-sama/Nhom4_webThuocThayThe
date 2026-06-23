namespace Nhom4WebThuocThayThe.Models;

public sealed class SavedDrug
{
    public int Id { get; set; }

    public required string UserEmail { get; set; }

    public int DrugId { get; set; }

    public DateTimeOffset SavedAt { get; set; }
}
