namespace Nhom4WebThuocThayThe.Models;

public sealed class ActiveIngredient
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Warning { get; set; }
}
