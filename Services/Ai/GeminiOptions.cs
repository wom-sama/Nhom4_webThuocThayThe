namespace Nhom4WebThuocThayThe.Services.Ai;

public sealed class GeminiOptions
{
    public const string SectionName = "AI:Gemini";

    public bool Enabled { get; set; }

    public string ApiKey { get; set; } = string.Empty;

    public string Model { get; set; } = "gemini-2.5-flash";

    public int TimeoutSeconds { get; set; } = 8;

    public int CacheMinutes { get; set; } = 15;

    public int MaxOutputTokens { get; set; } = 450;
}
