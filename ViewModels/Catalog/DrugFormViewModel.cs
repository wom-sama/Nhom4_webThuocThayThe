using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nhom4WebThuocThayThe.ViewModels.Catalog;

public sealed class DrugFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Strength { get; set; } = string.Empty;

    [Range(0, 100000000)]
    public decimal Price { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int DosageFormId { get; set; }

    [Required]
    public int UnitId { get; set; }

    [Required]
    public int ManufacturerId { get; set; }

    [Required]
    public int ActiveIngredientId { get; set; }

    [Required]
    [StringLength(50)]
    public string ActiveIngredientStrength { get; set; } = string.Empty;

    public bool PrescriptionRequired { get; set; }

    public bool IsActive { get; set; } = true;

    public string? Description { get; set; }

    public string? Usage { get; set; }

    public string? Contraindications { get; set; }

    public IEnumerable<SelectListItem> Categories { get; set; } = [];

    public IEnumerable<SelectListItem> DosageForms { get; set; } = [];

    public IEnumerable<SelectListItem> Units { get; set; } = [];

    public IEnumerable<SelectListItem> Manufacturers { get; set; } = [];

    public IEnumerable<SelectListItem> ActiveIngredients { get; set; } = [];
}
