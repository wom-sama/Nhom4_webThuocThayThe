namespace Nhom4WebThuocThayThe.Models;

public sealed class RegisteredUserAccount
{
    public required string Email { get; set; }

    public required string DisplayName { get; set; }

    public required string Role { get; set; }

    public required string PasswordSalt { get; set; }

    public required string PasswordHash { get; set; }

    public bool IsLocked { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
