using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nhom4WebThuocThayThe.ViewModels.Admin;

public sealed class AccountManagementViewModel
{
    public AccountCreateViewModel Create { get; set; } = new();

    public IReadOnlyCollection<AccountListItemViewModel> Accounts { get; set; } = [];

    public IEnumerable<SelectListItem> RoleOptions { get; set; } = [];
}
