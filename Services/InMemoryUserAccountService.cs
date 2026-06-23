using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.Services;

public sealed class InMemoryUserAccountService : IUserAccountService
{
    private readonly List<AppUser> _users;

    public InMemoryUserAccountService()
        : this(CreateDemoUsers())
    {
    }

    public InMemoryUserAccountService(IEnumerable<AppUser> users)
    {
        _users = users.ToList();
        if (_users.Count == 0)
        {
            throw new InvalidOperationException("At least one user account is required.");
        }
    }

    public static IReadOnlyCollection<AppUser> CreateDemoUsers() =>
    [
        new()
        {
            Email = "admin@nhom4.local",
            DisplayName = "Quản trị viên",
            Role = AppRoles.Admin,
            PasswordSalt = "6VKgNzqpI/qfApM5Q0g0ig==",
            PasswordHash = "el7XNeTksvc7i+AmuBloDjz1btG9tic4TttQgpl44uk="
        },
        new()
        {
            Email = "duocsi@nhom4.local",
            DisplayName = "Dược sĩ",
            Role = AppRoles.Pharmacist,
            PasswordSalt = "GJEYIeqZduWKh7WgLd8t5A==",
            PasswordHash = "gxH8oKXpAV9lZSlpIPTI+U3C0mb/XyfBWp3GyNU5+DQ="
        },
        new()
        {
            Email = "chuyengia@nhom4.local",
            DisplayName = "Chuyên gia y tế",
            Role = AppRoles.Expert,
            PasswordSalt = "70MZlMsHbFDBliFBLiciPw==",
            PasswordHash = "NBOdpiVV/OLQM1X4JAD85aPg0ybEF+dPwJ/EsonXqjg="
        },
        new()
        {
            Email = "user@nhom4.local",
            DisplayName = "Người dùng",
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
            PasswordHasher.Verify(password, user.PasswordSalt, user.PasswordHash));
    }

    public IReadOnlyCollection<AppUser> GetSeedUsers()
    {
        return _users.AsReadOnly();
    }

    public IReadOnlyCollection<UserAccountRecord> GetAccounts() =>
        _users
            .OrderBy(user => user.Role)
            .ThenBy(user => user.Email)
            .Select(user => UserAccountRecord.FromSeed(user))
            .ToList();

    public UserAccountOperationResult CreateAccount(
        string displayName,
        string email,
        string role,
        string password,
        string actor)
    {
        var normalizedEmail = UserAccountService.NormalizeEmail(email);
        if (_users.Any(user => string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase)))
        {
            return UserAccountOperationResult.Failure("Email này đã tồn tại.");
        }

        var (salt, hash) = PasswordHasher.Hash(password);
        _users.Add(new AppUser
        {
            Email = normalizedEmail,
            DisplayName = displayName.Trim(),
            Role = role,
            PasswordSalt = salt,
            PasswordHash = hash
        });
        return UserAccountOperationResult.Success("Đã tạo tài khoản.");
    }

    public UserAccountOperationResult RegisterUser(string displayName, string email, string password) =>
        CreateAccount(displayName, email, AppRoles.User, password, email);

    public UserAccountOperationResult SetLocked(string email, bool isLocked, string actor) =>
        UserAccountOperationResult.Failure("Tài khoản seed không hỗ trợ khóa trạng thái trong bộ nhớ.");

    public UserAccountOperationResult ResetPassword(string email, string newPassword, string actor) =>
        UserAccountOperationResult.Failure("Tài khoản seed không hỗ trợ đặt lại mật khẩu trong bộ nhớ.");
}
