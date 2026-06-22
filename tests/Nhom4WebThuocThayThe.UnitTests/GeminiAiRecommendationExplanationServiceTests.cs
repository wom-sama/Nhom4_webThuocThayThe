using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Nhom4WebThuocThayThe.Services.Ai;

namespace Nhom4WebThuocThayThe.UnitTests;

public sealed class GeminiAiRecommendationExplanationServiceTests
{
    [Fact]
    public async Task DisabledProviderReturnsDeterministicFallbackWithoutCallingHttp()
    {
        var handler = new StubHandler(_ => throw new InvalidOperationException("HTTP must not be called."));
        var service = CreateService(handler, new GeminiOptions { Enabled = false });

        var result = await service.ExplainAsync(CreateContext());

        Assert.False(result.IsAiGenerated);
        Assert.Equal("Deterministic fallback", result.Provider);
        Assert.Contains("78/100", result.Summary);
        Assert.Equal(2, result.Checkpoints.Count);
        Assert.Equal(0, handler.CallCount);
    }

    [Fact]
    public async Task SuccessfulStructuredResponseIsValidatedAndReturned()
    {
        var structured = JsonSerializer.Serialize(new
        {
            summary = "Ung vien trung khop hoat chat va ham luong.",
            checkpoints = new[] { "Con hang.", "Can duoc si xac nhan." },
            limitations = "Khong thay the tu van chuyen mon."
        });
        var envelope = JsonSerializer.Serialize(new
        {
            candidates = new[]
            {
                new { content = new { parts = new[] { new { text = structured } } } }
            }
        });
        var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(envelope, Encoding.UTF8, "application/json")
        });
        var service = CreateService(handler, EnabledOptions());

        var result = await service.ExplainAsync(CreateContext());

        Assert.True(result.IsAiGenerated);
        Assert.Equal("Google Gemini", result.Provider);
        Assert.Equal("gemini-2.5-flash-lite", result.Model);
        Assert.Equal(2, result.Checkpoints.Count);
        Assert.Equal(1, handler.CallCount);
        Assert.DoesNotContain("x-goog-api-key", handler.LastRequestHeaders, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-lite:generateContent?key=test-api-key",
            handler.LastRequestUri);
    }

    [Fact]
    public async Task TransientProviderFailureIsRetriedBeforeReturningAiResult()
    {
        var structured = JsonSerializer.Serialize(new
        {
            summary = "Ung vien phu hop.",
            checkpoints = new[] { "Con hang." },
            limitations = "Can duoc si xac nhan."
        });
        var envelope = JsonSerializer.Serialize(new
        {
            candidates = new[]
            {
                new { content = new { parts = new[] { new { text = structured } } } }
            }
        });
        var providerCalls = 0;
        var handler = new StubHandler(_ =>
        {
            providerCalls++;
            return providerCalls == 1
                ? new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                : new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(envelope, Encoding.UTF8, "application/json")
                };
        });
        var options = EnabledOptions();
        options.MaxAttempts = 2;
        options.RetryBaseMilliseconds = 1;
        var service = CreateService(handler, options);

        var result = await service.ExplainAsync(CreateContext());

        Assert.True(result.IsAiGenerated);
        Assert.Equal("Google Gemini", result.Provider);
        Assert.Equal(2, handler.CallCount);
    }

    [Theory]
    [InlineData("Here is the JSON requested:\n{\"summary\":\"Hop le.\",\"checkpoints\":[\"Con hang.\"],\"limitations\":\"Can duoc si xac nhan.\"}")]
    [InlineData("```json\n{\"summary\":\"Hop le.\",\"checkpoints\":[\"Con hang.\"],\"limitations\":\"Can duoc si xac nhan.\"}\n```")]
    public async Task StructuredResponseCanBeExtractedFromProviderWrapperText(string providerText)
    {
        var envelope = JsonSerializer.Serialize(new
        {
            candidates = new[]
            {
                new { content = new { parts = new[] { new { text = providerText } } } }
            }
        });
        var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(envelope, Encoding.UTF8, "application/json")
        });
        var service = CreateService(handler, EnabledOptions());

        var result = await service.ExplainAsync(CreateContext());

        Assert.True(result.IsAiGenerated);
        Assert.Equal("Google Gemini", result.Provider);
        Assert.Equal("Hop le.", result.Summary);
        Assert.Single(result.Checkpoints);
    }

    [Theory]
    [InlineData(HttpStatusCode.TooManyRequests)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task ProviderFailureReturnsSafeFallback(HttpStatusCode statusCode)
    {
        var handler = new StubHandler(_ => new HttpResponseMessage(statusCode));
        var service = CreateService(handler, EnabledOptions());

        var result = await service.ExplainAsync(CreateContext());

        Assert.False(result.IsAiGenerated);
        Assert.Contains("rule-based", result.Summary, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task MalformedStructuredResponseReturnsSafeFallback()
    {
        const string envelope = "{\"candidates\":[{\"content\":{\"parts\":[{\"text\":\"not-json\"}]}}]}";
        var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(envelope, Encoding.UTF8, "application/json")
        });
        var service = CreateService(handler, EnabledOptions());

        var result = await service.ExplainAsync(CreateContext());

        Assert.False(result.IsAiGenerated);
        Assert.Equal("Deterministic fallback", result.Provider);
    }

    [Fact]
    public async Task RequestContainsGuardrailsAndNoPersonalProfileFields()
    {
        string? body = null;
        var handler = new StubHandler(request =>
        {
            body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
        });
        var service = CreateService(handler, EnabledOptions());

        await service.ExplainAsync(CreateContext());

        Assert.NotNull(body);
        using var payload = JsonDocument.Parse(body);
        var guardrail = payload.RootElement
            .GetProperty("systemInstruction")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();
        Assert.NotNull(guardrail);
        Assert.Contains("không thay đổi điểm", guardrail, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("JSON không tin cậy", guardrail, StringComparison.OrdinalIgnoreCase);
        var generationConfig = payload.RootElement.GetProperty("generationConfig");
        Assert.Equal("application/json", generationConfig.GetProperty("responseMimeType").GetString());
        Assert.True(generationConfig.TryGetProperty("responseSchema", out var responseSchema));
        Assert.Equal("object", responseSchema.GetProperty("type").GetString());
        Assert.False(generationConfig.TryGetProperty("responseFormat", out _));
        Assert.False(generationConfig.TryGetProperty("thinkingConfig", out _));
        Assert.DoesNotContain("email", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("patient", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("profile", body, StringComparison.OrdinalIgnoreCase);
    }

    private static GeminiAiRecommendationExplanationService CreateService(
        HttpMessageHandler handler,
        GeminiOptions options)
    {
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/models/")
        };
        return new GeminiAiRecommendationExplanationService(
            client,
            Options.Create(options),
            new MemoryCache(new MemoryCacheOptions()),
            NullLogger<GeminiAiRecommendationExplanationService>.Instance);
    }

    private static GeminiOptions EnabledOptions() => new()
    {
        Enabled = true,
        ApiKey = "test-api-key",
        Model = "gemini-2.5-flash-lite",
        MaxAttempts = 1,
        RetryBaseMilliseconds = 1
    };

    private static AiRecommendationContext CreateContext() => new(
        "Panadol",
        "500 mg",
        "Paracetamol STADA",
        "500 mg",
        "Paracetamol",
        "Vien nen",
        false,
        24,
        78,
        ["Cung hoat chat.", "Con hang."],
        ["Can duoc si xac nhan."]);

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        public int CallCount { get; private set; }

        public string LastRequestHeaders { get; private set; } = string.Empty;

        public string? LastRequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            CallCount++;
            LastRequestHeaders = request.Headers.ToString();
            LastRequestUri = request.RequestUri?.ToString();
            return Task.FromResult(respond(request));
        }
    }
}
