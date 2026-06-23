using System.Security.Cryptography;

namespace Nhom4WebThuocThayThe.Services;

internal static class PasswordHasher
{
    private const int PasswordHashIterations = 100_000;
    private const int PasswordHashBytes = 32;
    private const int PasswordSaltBytes = 16;

    public static (string Salt, string Hash) Hash(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(PasswordSaltBytes);
        var hashBytes = Rfc2898DeriveBytes.Pbkdf2(
            password,
            saltBytes,
            PasswordHashIterations,
            HashAlgorithmName.SHA256,
            PasswordHashBytes);

        return (Convert.ToBase64String(saltBytes), Convert.ToBase64String(hashBytes));
    }

    public static bool Verify(string password, string salt, string expectedHash)
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
