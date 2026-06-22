using System.ComponentModel.DataAnnotations;

namespace Nhom4WebThuocThayThe.ViewModels.Auth;

public sealed class LoginViewModel
{
    [Display(Name = "Auth.Email")]
    [Required(ErrorMessage = "Validation.Required")]
    [EmailAddress(ErrorMessage = "Validation.Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Auth.Password")]
    [Required(ErrorMessage = "Validation.Required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
