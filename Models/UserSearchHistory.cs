namespace Nhom4WebThuocThayThe.Models;

public sealed class UserSearchHistory
{
    public int Id { get; set; }

    public required string UserEmail { get; set; }

    public string? Keyword { get; set; }

    public int? CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public int ResultCount { get; set; }

    public DateTimeOffset SearchedAt { get; set; }
}
