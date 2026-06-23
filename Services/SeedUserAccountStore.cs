using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.Services;

public sealed class SeedUserAccountStore
{
    public SeedUserAccountStore(IEnumerable<AppUser> users)
    {
        Users = users.ToList().AsReadOnly();
        if (Users.Count == 0)
        {
            throw new InvalidOperationException("At least one user account is required.");
        }
    }

    public IReadOnlyCollection<AppUser> Users { get; }
}
