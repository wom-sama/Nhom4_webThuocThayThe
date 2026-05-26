using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.Services;

public interface IUserAccountService
{
    AppUser? ValidateCredentials(string email, string password);

    IReadOnlyCollection<AppUser> GetSeedUsers();
}
