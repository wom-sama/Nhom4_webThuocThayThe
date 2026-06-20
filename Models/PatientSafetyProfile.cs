namespace Nhom4WebThuocThayThe.Models;

public sealed class PatientSafetyProfile
{
    public required string Email { get; init; }

    public required string DisplayName { get; init; }

    public List<int> AllergyActiveIngredientIds { get; init; } = [];

    public string? ClinicalNote { get; init; }
}
