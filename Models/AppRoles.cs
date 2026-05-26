namespace Nhom4WebThuocThayThe.Models;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Pharmacist = "Pharmacist";
    public const string Expert = "Expert";
    public const string User = "User";

    public static readonly string[] All = [Admin, Pharmacist, Expert, User];
}
