namespace Nhom4WebThuocThayThe.Models;

public sealed class Manufacturer
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Country { get; set; }
}
