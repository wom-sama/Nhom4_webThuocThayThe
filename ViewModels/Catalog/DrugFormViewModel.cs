using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nhom4WebThuocThayThe.ViewModels.Catalog;

public sealed class DrugFormViewModel
{
    public int? Id { get; set; }

    [Display(Name = "Drug.Name")]
    [Required(ErrorMessage = "Validation.Required")]
    [StringLength(120, ErrorMessage = "Validation.StringLength")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Drug.Strength")]
    [Required(ErrorMessage = "Validation.Required")]
    [StringLength(50, ErrorMessage = "Validation.StringLength")]
    public string Strength { get; set; } = string.Empty;

    [Display(Name = "Drug.Price")]
    [Range(0, 100000000, ErrorMessage = "Validation.Range")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Validation.Required")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Validation.Required")]
    public int DosageFormId { get; set; }

    [Required(ErrorMessage = "Validation.Required")]
    public int UnitId { get; set; }

    [Required(ErrorMessage = "Validation.Required")]
    public int ManufacturerId { get; set; }

    [Required(ErrorMessage = "Validation.Required")]
    public int ActiveIngredientId { get; set; }

    [Required(ErrorMessage = "Validation.Required")]
    [StringLength(50, ErrorMessage = "Validation.StringLength")]
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
