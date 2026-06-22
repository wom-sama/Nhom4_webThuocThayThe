using System.Text;
using System.Text.Json;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;

namespace Nhom4WebThuocThayThe.UnitTests;

public sealed class ConfiguredUserAccountLoaderTests
{
    [Fact]
    public void Load_AcceptsValidEncodedAccounts()
    {
        var account = new AppUser
        {
            Email = "production-admin@example.test",
            DisplayName = "Production admin",
            Role = AppRoles.Admin,
            PasswordSalt = Convert.ToBase64String(new byte[16]),
            PasswordHash = Convert.ToBase64String(new byte[32])
        };
        var encoded = Convert.ToBase64String(
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new[] { account })));

        var result = ConfiguredUserAccountLoader.Load(encoded);

        Assert.Single(result);
        Assert.Equal(AppRoles.Admin, result.Single().Role);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-base64")]
    public void Load_FailsClosedForMissingOrInvalidConfiguration(string? encoded)
    {
        Assert.Throws<InvalidOperationException>(() => ConfiguredUserAccountLoader.Load(encoded));
    }
}
