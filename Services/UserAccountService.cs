using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.Services;

public sealed class UserAccountService(
    PharmacyDbContext dbContext,
    SeedUserAccountStore seedUserAccountStore,
    IAuditLogService auditLogService) : IUserAccountService
{
    public AppUser? ValidateCredentials(string email, string password)
    {
        var normalizedEmail = NormalizeEmail(email);
        if (string.IsNullOrWhiteSpace(normalizedEmail) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var managedUser = dbContext.RegisteredUserAccounts
            .AsNoTracking()
            .FirstOrDefault(user => user.Email == normalizedEmail);
        if (managedUser is not null)
        {
            return !managedUser.IsLocked &&
                   PasswordHasher.Verify(password, managedUser.PasswordSalt, managedUser.PasswordHash)
                ? ToAppUser(managedUser)
                : null;
        }

        return seedUserAccountStore.Users.FirstOrDefault(user =>
            !user.IsLocked &&
            string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase) &&
            PasswordHasher.Verify(password, user.PasswordSalt, user.PasswordHash));
    }

    public IReadOnlyCollection<AppUser> GetSeedUsers() => seedUserAccountStore.Users;

    public IReadOnlyCollection<UserAccountRecord> GetAccounts()
    {
        var seedUsers = seedUserAccountStore.Users
            .Select(UserAccountRecord.FromSeed);
        var managedUsers = dbContext.RegisteredUserAccounts
            .AsNoTracking()
            .OrderBy(user => user.Role)
            .ThenBy(user => user.Email)
            .AsEnumerable()
            .Select(UserAccountRecord.FromManaged);

        return seedUsers
            .Concat(managedUsers)
            .OrderBy(user => user.Source)
            .ThenBy(user => user.Role)
            .ThenBy(user => user.Email)
            .ToList();
    }

    public UserAccountOperationResult RegisterUser(string displayName, string email, string password) =>
        CreateAccount(displayName, email, AppRoles.User, password, "self-registration");

    public UserAccountOperationResult CreateAccount(
        string displayName,
        string email,
        string role,
        string password,
        string actor)
    {
        var normalizedEmail = NormalizeEmail(email);
        var normalizedDisplayName = displayName.Trim();
        if (string.IsNullOrWhiteSpace(normalizedDisplayName))
        {
            return UserAccountOperationResult.Failure("Tên hiển thị không được để trống.");
        }

        if (string.IsNullOrWhiteSpace(normalizedEmail) || !normalizedEmail.Contains('@', StringComparison.Ordinal))
        {
            return UserAccountOperationResult.Failure("Email không hợp lệ.");
        }

        if (!AppRoles.All.Contains(role))
        {
            return UserAccountOperationResult.Failure("Vai trò không hợp lệ.");
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            return UserAccountOperationResult.Failure("Mật khẩu cần tối thiểu 8 ký tự.");
        }

        if (seedUserAccountStore.Users.Any(user => string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase)) ||
            dbContext.RegisteredUserAccounts.Any(user => user.Email == normalizedEmail))
        {
            return UserAccountOperationResult.Failure("Email này đã tồn tại.");
        }

        var (salt, hash) = PasswordHasher.Hash(password);
        var now = DateTimeOffset.UtcNow;
        dbContext.RegisteredUserAccounts.Add(new RegisteredUserAccount
        {
            Email = normalizedEmail,
            DisplayName = normalizedDisplayName,
            Role = role,
            PasswordSalt = salt,
            PasswordHash = hash,
            CreatedAt = now,
            UpdatedAt = now
        });
        dbContext.SaveChanges();
        auditLogService.Add(actor, "CreateAccount", "RegisteredUserAccount", $"{normalizedEmail} ({role})");

        return UserAccountOperationResult.Success("Đã tạo tài khoản.");
    }

    public UserAccountOperationResult SetLocked(string email, bool isLocked, string actor)
    {
        var normalizedEmail = NormalizeEmail(email);
        var user = dbContext.RegisteredUserAccounts.FirstOrDefault(item => item.Email == normalizedEmail);
        if (user is null)
        {
            return UserAccountOperationResult.Failure("Chỉ có tài khoản tạo trong hệ thống mới hỗ trợ khóa/mở khóa.");
        }

        if (user.IsLocked == isLocked)
        {
            return UserAccountOperationResult.Success("Trạng thái tài khoản không thay đổi.");
        }

        user.IsLocked = isLocked;
        user.UpdatedAt = DateTimeOffset.UtcNow;
        dbContext.SaveChanges();
        auditLogService.Add(actor, isLocked ? "LockAccount" : "UnlockAccount", "RegisteredUserAccount", normalizedEmail);

        return UserAccountOperationResult.Success(isLocked ? "Đã khóa tài khoản." : "Đã mở khóa tài khoản.");
    }

    public UserAccountOperationResult ResetPassword(string email, string newPassword, string actor)
    {
        var normalizedEmail = NormalizeEmail(email);
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
        {
            return UserAccountOperationResult.Failure("Mật khẩu mới cần tối thiểu 8 ký tự.");
        }

        var user = dbContext.RegisteredUserAccounts.FirstOrDefault(item => item.Email == normalizedEmail);
        if (user is null)
        {
            return UserAccountOperationResult.Failure("Chỉ có tài khoản tạo trong hệ thống mới hỗ trợ đặt lại mật khẩu.");
        }

        var (salt, hash) = PasswordHasher.Hash(newPassword);
        user.PasswordSalt = salt;
        user.PasswordHash = hash;
        user.UpdatedAt = DateTimeOffset.UtcNow;
        dbContext.SaveChanges();
        auditLogService.Add(actor, "ResetPassword", "RegisteredUserAccount", normalizedEmail);

        return UserAccountOperationResult.Success("Đã đặt lại mật khẩu.");
    }

    public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static AppUser ToAppUser(RegisteredUserAccount user) => new()
    {
        Email = user.Email,
        DisplayName = user.DisplayName,
        Role = user.Role,
        PasswordSalt = user.PasswordSalt,
        PasswordHash = user.PasswordHash,
        IsLocked = user.IsLocked
    };
}
