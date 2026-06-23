namespace Nhom4WebThuocThayThe.ViewModels.User;

public sealed class UserSearchHistoryItemViewModel
{
    public int Id { get; set; }

    public string? Keyword { get; set; }

    public int? CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public int ResultCount { get; set; }

    public DateTimeOffset SearchedAt { get; set; }
}
