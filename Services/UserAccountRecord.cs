using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.Services;

public sealed record UserAccountRecord(
    string Email,
    string DisplayName,
    string Role,
    bool IsLocked,
    string Source,
    bool CanManage,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt)
{
    public static UserAccountRecord FromSeed(AppUser user) =>
        new(
            user.Email,
            user.DisplayName,
            user.Role,
            user.IsLocked,
            "Seed",
            false,
            null,
            null);

    public static UserAccountRecord FromManaged(RegisteredUserAccount user) =>
        new(
            user.Email,
            user.DisplayName,
            user.Role,
            user.IsLocked,
            "Database",
            true,
            user.CreatedAt,
            user.UpdatedAt);
}
