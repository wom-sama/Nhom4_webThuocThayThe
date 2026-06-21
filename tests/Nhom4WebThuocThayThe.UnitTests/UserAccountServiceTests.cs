using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;

namespace Nhom4WebThuocThayThe.UnitTests;

public sealed class UserAccountServiceTests
{
    private readonly InMemoryUserAccountService _service = new();

    [Theory]
    [InlineData("admin@nhom4.local", "Admin@123", AppRoles.Admin)]
    [InlineData("duocsi@nhom4.local", "Duocsi@123", AppRoles.Pharmacist)]
    [InlineData("chuyengia@nhom4.local", "Chuyengia@123", AppRoles.Expert)]
    [InlineData("user@nhom4.local", "User@123", AppRoles.User)]
    public void ValidateCredentials_AcceptsKnownHashedAccounts(string email, string password, string role)
    {
        var user = _service.ValidateCredentials(email, password);

        Assert.NotNull(user);
        Assert.Equal(role, user.Role);
    }

    [Fact]
    public void ValidateCredentials_NormalizesEmail()
    {
        var user = _service.ValidateCredentials("  ADMIN@NHOM4.LOCAL  ", "Admin@123");

        Assert.NotNull(user);
        Assert.Equal(AppRoles.Admin, user.Role);
    }

    [Theory]
    [InlineData("admin@nhom4.local", "wrong")]
    [InlineData("missing@nhom4.local", "Admin@123")]
    public void ValidateCredentials_RejectsInvalidInput(string email, string password)
    {
        Assert.Null(_service.ValidateCredentials(email, password));
    }
}
