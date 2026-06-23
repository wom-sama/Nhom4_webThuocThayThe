using System.ComponentModel.DataAnnotations;

namespace Nhom4WebThuocThayThe.ViewModels.Admin;

public sealed class PasswordResetViewModel
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
    [MinLength(8, ErrorMessage = "Mật khẩu mới cần tối thiểu 8 ký tự.")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;
}
