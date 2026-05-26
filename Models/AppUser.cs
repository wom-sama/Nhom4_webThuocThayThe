namespace Nhom4WebThuocThayThe.Models;

public sealed class AppUser
{
    public required string Email { get; init; }

    public required string DisplayName { get; init; }

    public required string Role { get; init; }

    public required string Password { get; init; }

    public bool IsLocked { get; init; }
}
