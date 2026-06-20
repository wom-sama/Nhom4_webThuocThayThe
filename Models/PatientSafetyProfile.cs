using System.ComponentModel.DataAnnotations.Schema;

namespace Nhom4WebThuocThayThe.Models;

public sealed class PatientSafetyProfile
{
    public required string Email { get; set; }

    public required string DisplayName { get; set; }

    public string AllergyActiveIngredientIdsCsv { get; set; } = string.Empty;

    public string? ClinicalNote { get; set; }

    [NotMapped]
    public IReadOnlyCollection<int> AllergyActiveIngredientIds =>
        AllergyActiveIngredientIdsCsv
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(value => int.TryParse(value, out var id) ? id : 0)
            .Where(id => id > 0)
            .ToList();
}
