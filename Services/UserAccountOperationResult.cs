namespace Nhom4WebThuocThayThe.Services;

public sealed record UserAccountOperationResult(bool IsSuccess, string Message)
{
    public static UserAccountOperationResult Success(string message) => new(true, message);

    public static UserAccountOperationResult Failure(string message) => new(false, message);
}
