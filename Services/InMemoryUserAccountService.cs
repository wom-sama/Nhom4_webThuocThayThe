using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.Services;

public sealed class InMemoryUserAccountService : IUserAccountService
{
    private readonly List<AppUser> _users =
    [
        new()
        {
            Email = "admin@nhom4.local",
            DisplayName = "Quan tri vien",
            Role = AppRoles.Admin,
            Password = "Admin@123"
        },
        new()
        {
            Email = "duocsi@nhom4.local",
            DisplayName = "Duoc si",
            Role = AppRoles.Pharmacist,
            Password = "Duocsi@123"
        },
        new()
        {
            Email = "chuyengia@nhom4.local",
            DisplayName = "Chuyen gia y te",
            Role = AppRoles.Expert,
            Password = "Chuyengia@123"
        },
        new()
        {
            Email = "user@nhom4.local",
            DisplayName = "Nguoi dung",
            Role = AppRoles.User,
            Password = "User@123"
        }
    ];

    public AppUser? ValidateCredentials(string email, string password)
    {
        return _users.FirstOrDefault(user =>
            !user.IsLocked &&
            string.Equals(user.Email, email.Trim(), StringComparison.OrdinalIgnoreCase) &&
            user.Password == password);
    }

    public IReadOnlyCollection<AppUser> GetSeedUsers()
    {
        return _users.AsReadOnly();
    }
}
