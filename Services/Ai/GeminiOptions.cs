namespace Nhom4WebThuocThayThe.Services.Ai;

public sealed class GeminiOptions
{
    public const string SectionName = "AI:Gemini";

    public bool Enabled { get; set; }

    public string ApiKey { get; set; } = string.Empty;

    public string Model { get; set; } = "gemini-2.5-flash-lite";

    public int TimeoutSeconds { get; set; } = 20;

    public int CacheMinutes { get; set; } = 30;

    public int MaxOutputTokens { get; set; } = 320;

    public int MaxAttempts { get; set; } = 3;

    public int RetryBaseMilliseconds { get; set; } = 750;
}
