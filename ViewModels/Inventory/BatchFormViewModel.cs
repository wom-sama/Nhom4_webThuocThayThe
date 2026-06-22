using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nhom4WebThuocThayThe.ViewModels.Inventory;

public sealed class BatchFormViewModel
{
    [Required(ErrorMessage = "Validation.Required")]
    public int DrugId { get; set; }

    [Required(ErrorMessage = "Validation.Required")]
    public int WarehouseId { get; set; }

    [Required(ErrorMessage = "Validation.Required")]
    [StringLength(40, ErrorMessage = "Validation.StringLength")]
    public string BatchNumber { get; set; } = string.Empty;

    [Range(0, 1000000, ErrorMessage = "Validation.Range")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Validation.Required")]
    [DataType(DataType.Date)]
    public DateOnly ImportedDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required(ErrorMessage = "Validation.Required")]
    [DataType(DataType.Date)]
    public DateOnly ExpiryDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddYears(1));

    public IEnumerable<SelectListItem> Drugs { get; set; } = [];

    public IEnumerable<SelectListItem> Warehouses { get; set; } = [];
}
