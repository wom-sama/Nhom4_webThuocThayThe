namespace Nhom4WebThuocThayThe.ViewModels.Admin;

public sealed class AccountListItemViewModel
{
    public required string Email { get; set; }

    public required string DisplayName { get; set; }

    public required string Role { get; set; }

    public required string Source { get; set; }

    public bool IsLocked { get; set; }

    public bool CanManage { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
