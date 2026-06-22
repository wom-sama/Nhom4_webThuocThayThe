using System.Text;
using System.Text.Json;
using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.Services;

public static class ConfiguredUserAccountLoader
{
    public static IReadOnlyCollection<AppUser> Load(string? encodedAccounts)
    {
        if (string.IsNullOrWhiteSpace(encodedAccounts))
        {
            throw new InvalidOperationException(
                "Production authentication is not configured. Set Authentication__EncodedAccounts.");
        }

        try
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(encodedAccounts));
            var accounts = JsonSerializer.Deserialize<List<AppUser>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
            Validate(accounts);
            return accounts;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception exception) when (exception is FormatException or JsonException)
        {
            throw new InvalidOperationException("Production authentication configuration is invalid.", exception);
        }
    }

    private static void Validate(IReadOnlyCollection<AppUser> accounts)
    {
        if (accounts.Count == 0)
        {
            throw new InvalidOperationException("Production authentication must contain at least one account.");
        }

        if (accounts.Select(item => item.Email).Distinct(StringComparer.OrdinalIgnoreCase).Count() != accounts.Count)
        {
            throw new InvalidOperationException("Production authentication contains duplicate email addresses.");
        }

        foreach (var account in accounts)
        {
            if (string.IsNullOrWhiteSpace(account.Email) ||
                string.IsNullOrWhiteSpace(account.DisplayName) ||
                !AppRoles.All.Contains(account.Role, StringComparer.Ordinal) ||
                !HasDecodedLength(account.PasswordSalt, 16) ||
                !HasDecodedLength(account.PasswordHash, 32))
            {
                throw new InvalidOperationException("Production authentication contains an invalid account.");
            }
        }
    }

    private static bool HasDecodedLength(string value, int expectedLength)
    {
        try
        {
            return Convert.FromBase64String(value).Length == expectedLength;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
