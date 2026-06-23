using System.ComponentModel.DataAnnotations;
using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.ViewModels.Admin;

public sealed class AccountCreateViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [Display(Name = "Họ tên")]
    [StringLength(120)]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Vai trò")]
    public string Role { get; set; } = AppRoles.User;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [MinLength(8, ErrorMessage = "Mật khẩu cần tối thiểu 8 ký tự.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
