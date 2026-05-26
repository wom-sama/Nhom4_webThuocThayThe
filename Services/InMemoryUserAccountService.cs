using System.Security.Cryptography;
using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.Services;

public sealed class InMemoryUserAccountService : IUserAccountService
{
    private const int PasswordHashIterations = 100_000;
    private const int PasswordHashBytes = 32;

    private readonly List<AppUser> _users =
    [
        new()
        {
            Email = "admin@nhom4.local",
            DisplayName = "Quan tri vien",
            Role = AppRoles.Admin,
            PasswordSalt = "6VKgNzqpI/qfApM5Q0g0ig==",
            PasswordHash = "el7XNeTksvc7i+AmuBloDjz1btG9tic4TttQgpl44uk="
        },
        new()
        {
            Email = "duocsi@nhom4.local",
            DisplayName = "Duoc si",
            Role = AppRoles.Pharmacist,
            PasswordSalt = "GJEYIeqZduWKh7WgLd8t5A==",
            PasswordHash = "gxH8oKXpAV9lZSlpIPTI+U3C0mb/XyfBWp3GyNU5+DQ="
        },
        new()
        {
            Email = "chuyengia@nhom4.local",
            DisplayName = "Chuyen gia y te",
            Role = AppRoles.Expert,
            PasswordSalt = "70MZlMsHbFDBliFBLiciPw==",
            PasswordHash = "NBOdpiVV/OLQM1X4JAD85aPg0ybEF+dPwJ/EsonXqjg="
        },
        new()
        {
            Email = "user@nhom4.local",
            DisplayName = "Nguoi dung",
            Role = AppRoles.User,
            PasswordSalt = "FlkBYh0kzwbSw9YYW2SVog==",
            PasswordHash = "LFQmL72UMIGthgrqk7IdaqV2pI/6OA+ncmTZUeXUZr4="
        }
    ];

    public AppUser? ValidateCredentials(string email, string password)
    {
        return _users.FirstOrDefault(user =>
            !user.IsLocked &&
            string.Equals(user.Email, email.Trim(), StringComparison.OrdinalIgnoreCase) &&
            VerifyPassword(password, user.PasswordSalt, user.PasswordHash));
    }

    public IReadOnlyCollection<AppUser> GetSeedUsers()
    {
        return _users.AsReadOnly();
    }

    private static bool VerifyPassword(string password, string salt, string expectedHash)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var expectedBytes = Convert.FromBase64String(expectedHash);
        var actualBytes = Rfc2898DeriveBytes.Pbkdf2(
            password,
            saltBytes,
            PasswordHashIterations,
            HashAlgorithmName.SHA256,
            PasswordHashBytes);

        return CryptographicOperations.FixedTimeEquals(actualBytes, expectedBytes);
    }
}
