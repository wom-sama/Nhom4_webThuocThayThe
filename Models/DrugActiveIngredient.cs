namespace Nhom4WebThuocThayThe.Models;

public sealed class DrugActiveIngredient
{
    public int DrugId { get; set; }

    public int ActiveIngredientId { get; set; }

    public required string Strength { get; set; }
}
