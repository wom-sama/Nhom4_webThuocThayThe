using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.Services;

public interface IUserAccountService
{
    AppUser? ValidateCredentials(string email, string password);

    IReadOnlyCollection<AppUser> GetSeedUsers();

    IReadOnlyCollection<UserAccountRecord> GetAccounts();

    UserAccountOperationResult RegisterUser(string displayName, string email, string password);

    UserAccountOperationResult CreateAccount(string displayName, string email, string role, string password, string actor);

    UserAccountOperationResult SetLocked(string email, bool isLocked, string actor);

    UserAccountOperationResult ResetPassword(string email, string newPassword, string actor);
}
