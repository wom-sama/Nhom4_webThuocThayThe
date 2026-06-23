using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.Services;

if (Environment.GetEnvironmentVariable("N4WTT_DIRECT_DB_DIAGNOSTIC") == "1")
{
    return DiagnoseReportingService();
}

var baseUrl = Environment.GetEnvironmentVariable("N4WTT_PRODUCTION_URL")
    ?? "https://nnhom4web.somee.com";
var outputPath = Environment.GetEnvironmentVariable("N4WTT_PRODUCTION_REPORT")
    ?? Path.Combine(Directory.GetCurrentDirectory(), "TestResults", "production-validation-report.json");

var credentials = new Dictionary<string, TestCredential>(StringComparer.OrdinalIgnoreCase)
{
    ["Admin"] = ReadCredential("ADMIN"),
    ["Pharmacist"] = ReadCredential("PHARMACIST"),
    ["Expert"] = ReadCredential("EXPERT"),
    ["User"] = ReadCredential("USER")
};

var tests = new List<ProductionTestResult>();
using var publicClient = CreateClient();

if (Environment.GetEnvironmentVariable("N4WTT_AUTH_SMOKE_ONLY") == "1")
{
    EnsureCredentials();
    using var adminClient = CreateClient();
    await Login(adminClient, credentials["Admin"]);
    using var adminResponse = await adminClient.GetAsync(new Uri(new Uri(baseUrl), "/Admin"));
    Expect(adminResponse.StatusCode == HttpStatusCode.OK, $"Admin smoke status={(int)adminResponse.StatusCode}");
    Console.WriteLine("Production-configured Admin authentication smoke passed.");
    return 0;
}

await Run("PROD01", "Health", "HTTPS health reports database connected", async () =>
{
    using var response = await publicClient.GetAsync(new Uri(new Uri(baseUrl), "/health"));
    var body = await response.Content.ReadAsStringAsync();
    Expect(response.StatusCode == HttpStatusCode.OK, $"status={(int)response.StatusCode}");
    Expect(body.Contains("\"database\":\"connected\"", StringComparison.OrdinalIgnoreCase), "database is not connected");
});

await Run("PROD02", "Public UI", "Home is Vietnamese and hides demo credentials", async () =>
{
    using var response = await publicClient.GetAsync(new Uri(new Uri(baseUrl), "/"));
    var html = await response.Content.ReadAsStringAsync();
    Expect(response.StatusCode == HttpStatusCode.OK, $"status={(int)response.StatusCode}");
    Expect(html.Contains("<html lang=\"vi\"", StringComparison.OrdinalIgnoreCase), "html lang is not vi");
    Expect(response.Content.Headers.ContentLanguage.Contains("vi-VN"), "Content-Language is not vi-VN");
    Expect(!ContainsDemoCredential(html), "demo credential leaked on home");
});

await Run("PROD03", "Search", "Public search and category filter return expected content", async () =>
{
    using var response = await publicClient.GetAsync(new Uri(new Uri(baseUrl), "/Drugs?keyword=Paracetamol&categoryId=2"));
    var html = await response.Content.ReadAsStringAsync();
    Expect(response.StatusCode == HttpStatusCode.OK, $"status={(int)response.StatusCode}");
    Expect(html.Contains("Paracetamol", StringComparison.OrdinalIgnoreCase), "filtered search result missing");
    Expect(!html.Contains("Amoxicillin 500mg", StringComparison.OrdinalIgnoreCase), "category filter did not narrow results");
});

await Run("PROD04", "Recommendation", "Out-of-stock detail exposes deterministic alternatives", async () =>
{
    using var response = await publicClient.GetAsync(new Uri(new Uri(baseUrl), "/Drugs/Details/1"));
    var html = await response.Content.ReadAsStringAsync();
    Expect(response.StatusCode == HttpStatusCode.OK, $"status={(int)response.StatusCode}");
    Expect(html.Contains("Hết hàng", StringComparison.OrdinalIgnoreCase), "out-of-stock state missing");
    Expect(html.Contains("Thuốc thay thế đề xuất", StringComparison.OrdinalIgnoreCase), "alternative section missing");
    Expect(html.Contains("Paracetamol DHG 500mg", StringComparison.OrdinalIgnoreCase), "expected candidate missing");
});

await Run("PROD05", "Headers", "Dynamic responses include browser hardening headers", async () =>
{
    using var response = await publicClient.GetAsync(new Uri(new Uri(baseUrl), "/Auth/Login"));
    Expect(response.Headers.TryGetValues("Content-Security-Policy", out _), "CSP missing");
    Expect(HeaderEquals(response, "X-Content-Type-Options", "nosniff"), "X-Content-Type-Options missing");
    Expect(HeaderEquals(response, "X-Frame-Options", "DENY"), "X-Frame-Options missing");
    Expect(HeaderEquals(response, "Referrer-Policy", "no-referrer"), "Referrer-Policy missing");
    Expect(response.Headers.TryGetValues("Strict-Transport-Security", out _), "HSTS missing on HTTPS");
});

await Run("PROD06", "Cache", "Static stylesheet uses public cache metadata", async () =>
{
    using var response = await publicClient.GetAsync(new Uri(new Uri(baseUrl), "/css/site.css"));
    var cache = response.Headers.CacheControl?.ToString() ?? string.Empty;
    Expect(response.StatusCode == HttpStatusCode.OK, $"status={(int)response.StatusCode}");
    Expect(cache.Contains("public", StringComparison.OrdinalIgnoreCase), "public cache directive missing");
    Expect(cache.Contains("max-age=604800", StringComparison.OrdinalIgnoreCase), "seven-day max-age missing");
});

await Run("PROD07", "Auth", "Invalid login does not authenticate or set auth cookie", async () =>
{
    using var client = CreateClient();
    var login = await client.GetStringAsync(new Uri(new Uri(baseUrl), "/Auth/Login"));
    var token = GetAntiForgeryToken(login);
    using var response = await client.PostAsync(
        new Uri(new Uri(baseUrl), "/Auth/Login"),
        Form(new Dictionary<string, string>
        {
            ["Email"] = "invalid@example.test",
            ["Password"] = "invalid-password",
            ["ReturnUrl"] = string.Empty,
            ["__RequestVerificationToken"] = token
        }));
    var body = await response.Content.ReadAsStringAsync();
    Expect(response.StatusCode == HttpStatusCode.OK, $"status={(int)response.StatusCode}");
    Expect(body.Contains("validation-summary-errors", StringComparison.OrdinalIgnoreCase), "generic invalid-login error missing");
    Expect(!response.Headers.TryGetValues("Set-Cookie", out var cookies) ||
           cookies.All(item => !item.Contains("N4WTT.Auth", StringComparison.OrdinalIgnoreCase)),
        "authentication cookie set for invalid login");
});

await Run("PROD08", "RBAC", "Anonymous requests cannot open role Areas", async () =>
{
    foreach (var area in new[] { "/Admin", "/Pharmacist", "/Expert", "/User" })
    {
        using var response = await publicClient.GetAsync(new Uri(new Uri(baseUrl), area));
        Expect(response.StatusCode == HttpStatusCode.Redirect, $"{area} status={(int)response.StatusCode}");
        Expect(response.Headers.Location?.OriginalString.Contains("/Auth/Login", StringComparison.OrdinalIgnoreCase) == true,
            $"{area} did not redirect to login");
    }
});

await Run("PROD09", "RBAC", "Every test role is isolated to its assigned Area", async () =>
{
    EnsureCredentials();
    var areas = new[] { "Admin", "Pharmacist", "Expert", "User" };
    var failures = new List<string>();
    foreach (var role in areas)
    {
        using var client = CreateClient();
        await Login(client, credentials[role]);
        foreach (var target in areas)
        {
            using var response = await client.GetAsync(new Uri(new Uri(baseUrl), $"/{target}"));
            if (role.Equals(target, StringComparison.OrdinalIgnoreCase))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    failures.Add($"{role}->{target} status={(int)response.StatusCode}");
                }
            }
            else if (response.StatusCode is not HttpStatusCode.Redirect and not HttpStatusCode.Forbidden)
            {
                failures.Add($"{role}->{target} status={(int)response.StatusCode}");
            }
        }
    }

    Expect(failures.Count == 0, string.Join("; ", failures));
});

await Run("PROD10", "CSRF", "Protected write without anti-forgery token is rejected", async () =>
{
    EnsureCredentials();
    using var client = CreateClient();
    await Login(client, credentials["Admin"]);
    using var response = await client.PostAsync(
        new Uri(new Uri(baseUrl), "/Admin/ExternalData/MarkSynced"),
        Form(new Dictionary<string, string> { ["id"] = "1" }));
    Expect(response.StatusCode == HttpStatusCode.BadRequest, $"status={(int)response.StatusCode}");
});

await Run("PROD11", "Security", "Search input is encoded and unknown routes hide internals", async () =>
{
    const string payload = "<script>alert(1)</script>";
    using var search = await publicClient.GetAsync(
        new Uri(new Uri(baseUrl), "/Drugs?keyword=" + Uri.EscapeDataString(payload)));
    var searchHtml = await search.Content.ReadAsStringAsync();
    Expect(!searchHtml.Contains(payload, StringComparison.Ordinal), "raw script reflected");
    Expect(searchHtml.Contains("&lt;script&gt;", StringComparison.OrdinalIgnoreCase), "encoded payload missing");

    using var missing = await publicClient.GetAsync(new Uri(new Uri(baseUrl), "/missing-production-probe"));
    var missingHtml = await missing.Content.ReadAsStringAsync();
    Expect(missing.StatusCode == HttpStatusCode.NotFound, $"missing status={(int)missing.StatusCode}");
    Expect(!missingHtml.Contains("StackTrace", StringComparison.OrdinalIgnoreCase), "stack trace leaked");
    Expect(!missingHtml.Contains("SqlException", StringComparison.OrdinalIgnoreCase), "SQL exception leaked");
});

await Run("PROD12", "AI security", "AI endpoint rejects unsafe methods and browser output hides the key", async () =>
{
    using var get = await publicClient.GetAsync(new Uri(new Uri(baseUrl), "/Drugs/ExplainAlternative"));
    Expect(get.StatusCode == HttpStatusCode.MethodNotAllowed, $"GET status={(int)get.StatusCode}");

    using var post = await publicClient.PostAsync(
        new Uri(new Uri(baseUrl), "/Drugs/ExplainAlternative"),
        Form(new Dictionary<string, string> { ["sourceId"] = "1", ["candidateId"] = "2" }));
    Expect(post.StatusCode == HttpStatusCode.BadRequest, $"POST status={(int)post.StatusCode}");

    var details = await publicClient.GetStringAsync(new Uri(new Uri(baseUrl), "/Drugs/Details/1"));
    var script = await publicClient.GetStringAsync(new Uri(new Uri(baseUrl), "/js/site.js"));
    Expect(!details.Contains("AIza", StringComparison.Ordinal), "possible API key in detail HTML");
    Expect(!script.Contains("AIza", StringComparison.Ordinal), "possible API key in browser JavaScript");
});

await Run("PROD13", "Performance", "Paced production reads stay within the Somee-safe baseline", async () =>
{
    var samples = new List<double>();
    var errors = 0;
    var paths = new[] { "/", "/Drugs", "/Drugs/Details/1", "/health", "/css/site.css" };
    for (var index = 0; index < 30; index++)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            using var response = await publicClient.GetAsync(new Uri(new Uri(baseUrl), paths[index % paths.Length]));
            if (!response.IsSuccessStatusCode)
            {
                errors++;
            }
        }
        catch
        {
            errors++;
        }
        finally
        {
            stopwatch.Stop();
            samples.Add(stopwatch.Elapsed.TotalMilliseconds);
        }

        await Task.Delay(250);
    }

    samples.Sort();
    var p50 = Percentile(samples, 0.50);
    var p95 = Percentile(samples, 0.95);
    var max = samples[^1];
    Expect(errors == 0, $"errors={errors}");
    Expect(p95 <= 2500, $"p95={p95:N1}ms");
    tests.LastOrDefault()?.Metrics.Add("p50Ms", Math.Round(p50, 2));
    tests.LastOrDefault()?.Metrics.Add("p95Ms", Math.Round(p95, 2));
    tests.LastOrDefault()?.Metrics.Add("maxMs", Math.Round(max, 2));
});

await Run("PROD14", "Role UI", "Expert and Pharmacist production routes render distinct usable screens", async () =>
{
    EnsureCredentials();
    using var expertClient = CreateClient();
    await Login(expertClient, credentials["Expert"]);
    var pending = await GetOk(expertClient, "/Expert/Reviews");
    var evidence = await GetOk(expertClient, "/Expert/Reviews/Evidence");
    var history = await GetOk(expertClient, "/Expert/Reviews/History");
    Expect(pending.Contains("Hồ sơ cần quyết định"), "expert pending title missing");
    Expect(pending.Contains("review-form"), "expert pending form missing");
    Expect(evidence.Contains("Cơ sở xếp hạng đề xuất"), "expert evidence title missing");
    Expect(evidence.Contains("evidence-grid"), "expert evidence grid missing");
    Expect(history.Contains("Lịch sử quyết định"), "expert history title missing");
    Expect(history.Contains("timeline-list"), "expert history timeline missing");

    using var pharmacistClient = CreateClient();
    await Login(pharmacistClient, credentials["Pharmacist"]);
    var queue = await GetOk(pharmacistClient, "/Pharmacist/Workspace");
    var search = await GetOk(pharmacistClient, "/Pharmacist/Workspace/Search?keyword=Loratadine");
    var compare = await GetOk(pharmacistClient, "/Pharmacist/Workspace/Compare");
    Expect(queue.Contains("Thuốc cần phương án thay thế"), "pharmacist queue title missing");
    Expect(queue.Contains("queue-card"), "pharmacist queue cards missing");
    Expect(search.Contains("Loratadine 10mg"), "pharmacist search seed result missing");
    Expect(compare.Contains("Ứng viên theo điểm, tồn kho và cảnh báo"), "pharmacist compare title missing");
    Expect(compare.Contains("comparison-grid"), "pharmacist compare grid missing");
});

await Run("PROD15", "Content", "Production has expanded data and concrete privacy policy", async () =>
{
    var stomach = await GetOk(publicClient, "/Drugs?keyword=Omeprazole");
    var respiratory = await GetOk(publicClient, "/Drugs?keyword=Ventolin");
    var cardio = await GetOk(publicClient, "/Drugs?keyword=Amlodipine");
    var privacy = await GetOk(publicClient, "/Home/Privacy");

    Expect(stomach.Contains("Omeprazole STADA 20mg"), "Omeprazole seed missing on production");
    Expect(respiratory.Contains("Ventolin Inhaler 100mcg"), "Ventolin seed missing on production");
    Expect(cardio.Contains("Amlodipine 5mg"), "Amlodipine seed missing on production");
    Expect(privacy.Contains("Phạm vi dữ liệu"), "privacy data scope missing");
    Expect(privacy.Contains("Ranh giới AI"), "privacy AI boundary missing");
    Expect(privacy.Contains("Sự cố và liên hệ"), "privacy incident policy missing");
    Expect(!privacy.Contains("Use this page", StringComparison.OrdinalIgnoreCase), "privacy template copy still visible");
});

await Run("PROD16", "Gemini", "Gemini explanation endpoint is live or safely reported", async () =>
{
    var details = await GetOk(publicClient, "/Drugs/Details/1");
    var token = GetAntiForgeryToken(details);
    using var response = await publicClient.PostAsync(
        new Uri(new Uri(baseUrl), "/Drugs/ExplainAlternative"),
        Form(new Dictionary<string, string>
        {
            ["sourceId"] = "1",
            ["candidateId"] = "2",
            ["__RequestVerificationToken"] = token
        }));
    var json = await response.Content.ReadAsStringAsync();
    Expect(response.StatusCode == HttpStatusCode.OK, $"AI POST status={(int)response.StatusCode}");
    Expect(json.Contains("\"provider\"", StringComparison.OrdinalIgnoreCase), "AI response provider missing");
    Expect(!json.Contains("AIza", StringComparison.Ordinal), "AI response leaked API key");

    if (Environment.GetEnvironmentVariable("N4WTT_EXPECT_GEMINI_LIVE") == "1")
    {
        Expect(json.Contains("\"isAiGenerated\":true", StringComparison.OrdinalIgnoreCase), "Gemini did not return a live AI response");
        Expect(json.Contains("Google Gemini", StringComparison.OrdinalIgnoreCase), "Gemini provider label missing");
    }
});

Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputPath))!);
await File.WriteAllTextAsync(outputPath, JsonSerializer.Serialize(tests, new JsonSerializerOptions { WriteIndented = true }));

var passed = tests.Count(item => item.Status == "Pass");
foreach (var result in tests)
{
    Console.WriteLine($"{result.Status.ToUpperInvariant()} {result.Id} {result.Area}: {result.Title} ({result.ElapsedMilliseconds} ms){(result.Error is null ? string.Empty : $" - {result.Error}")}");
}
Console.WriteLine($"Production validation: {passed}/{tests.Count} passed");
Console.WriteLine($"Report: {Path.GetFullPath(outputPath)}");
return passed == tests.Count ? 0 : 1;

async Task Run(string id, string area, string title, Func<Task> action)
{
    var result = new ProductionTestResult(id, area, title);
    tests.Add(result);
    var stopwatch = Stopwatch.StartNew();
    try
    {
        await action();
        result.Status = "Pass";
    }
    catch (Exception exception)
    {
        result.Status = "Fail";
        result.Error = exception.Message;
    }
    finally
    {
        stopwatch.Stop();
        result.ElapsedMilliseconds = Math.Round(stopwatch.Elapsed.TotalMilliseconds, 2);
    }
}

HttpClient CreateClient()
{
    var handler = new HttpClientHandler
    {
        AllowAutoRedirect = false,
        CookieContainer = new CookieContainer(),
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli
    };
    return new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(45) };
}

async Task<string> GetOk(HttpClient client, string path)
{
    using var response = await client.GetAsync(new Uri(new Uri(baseUrl), path));
    var body = await response.Content.ReadAsStringAsync();
    Expect(response.StatusCode == HttpStatusCode.OK, $"{path} status={(int)response.StatusCode}");
    return WebUtility.HtmlDecode(body);
}

async Task Login(HttpClient client, TestCredential credential)
{
    var loginHtml = await client.GetStringAsync(new Uri(new Uri(baseUrl), "/Auth/Login"));
    var token = GetAntiForgeryToken(loginHtml);
    using var response = await client.PostAsync(
        new Uri(new Uri(baseUrl), "/Auth/Login"),
        Form(new Dictionary<string, string>
        {
            ["Email"] = credential.Email,
            ["Password"] = credential.Password,
            ["ReturnUrl"] = string.Empty,
            ["__RequestVerificationToken"] = token
        }));
    Expect(response.StatusCode == HttpStatusCode.Redirect, $"login status={(int)response.StatusCode}");
}

TestCredential ReadCredential(string role) => new(
    Environment.GetEnvironmentVariable($"N4WTT_TEST_{role}_EMAIL") ?? string.Empty,
    Environment.GetEnvironmentVariable($"N4WTT_TEST_{role}_PASSWORD") ?? string.Empty);

void EnsureCredentials()
{
    var missing = credentials.Where(item => !item.Value.IsConfigured).Select(item => item.Key).ToArray();
    Expect(missing.Length == 0, "missing production test credentials: " + string.Join(", ", missing));
}

static FormUrlEncodedContent Form(IReadOnlyDictionary<string, string> values) => new(values);

static string GetAntiForgeryToken(string html)
{
    var match = Regex.Match(
        html,
        "name=\"__RequestVerificationToken\"[^>]*value=\"(?<token>[^\"]+)\"|value=\"(?<token>[^\"]+)\"[^>]*name=\"__RequestVerificationToken\"",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    if (!match.Success)
    {
        throw new InvalidOperationException("anti-forgery token missing");
    }
    return WebUtility.HtmlDecode(match.Groups["token"].Value);
}

static bool HeaderEquals(HttpResponseMessage response, string name, string expected) =>
    response.Headers.TryGetValues(name, out var values) && values.Any(item => item.Equals(expected, StringComparison.OrdinalIgnoreCase));

static bool ContainsDemoCredential(string text) =>
    text.Contains("admin@nhom4.local", StringComparison.OrdinalIgnoreCase) ||
    text.Contains("duocsi@nhom4.local", StringComparison.OrdinalIgnoreCase) ||
    text.Contains("chuyengia@nhom4.local", StringComparison.OrdinalIgnoreCase) ||
    text.Contains("user@nhom4.local", StringComparison.OrdinalIgnoreCase) ||
    text.Contains("Admin@123", StringComparison.Ordinal) ||
    text.Contains("Duocsi@123", StringComparison.Ordinal) ||
    text.Contains("Chuyengia@123", StringComparison.Ordinal) ||
    text.Contains("User@123", StringComparison.Ordinal);

static double Percentile(IReadOnlyList<double> sorted, double percentile)
{
    var index = Math.Clamp((int)Math.Ceiling(sorted.Count * percentile) - 1, 0, sorted.Count - 1);
    return sorted[index];
}

static void Expect(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}

static int DiagnoseReportingService()
{
    var connectionString = Environment.GetEnvironmentVariable("N4WTT_SQL_CONNECTION_STRING");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        Console.Error.WriteLine("N4WTT_SQL_CONNECTION_STRING is required for direct DB diagnostics.");
        return 2;
    }

    try
    {
        var options = new DbContextOptionsBuilder<PharmacyDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        using var dbContext = new PharmacyDbContext(options);
        var inventory = new InventoryService(dbContext);
        var recommendations = new RecommendationService(dbContext);
        var audit = new AuditLogService(dbContext);
        var reporting = new ReportingService(dbContext, inventory, recommendations, audit);
        var dashboard = reporting.GetDashboard();
        Console.WriteLine($"Dashboard diagnostic passed: metrics={dashboard.Metrics.Count}, risks={dashboard.StockRisks.Count}.");
        return 0;
    }
    catch (Exception exception)
    {
        Console.Error.WriteLine(exception);
        return 1;
    }
}

sealed record TestCredential(string Email, string Password)
{
    public bool IsConfigured => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
}

sealed class ProductionTestResult(string id, string area, string title)
{
    public string Id { get; } = id;
    public string Area { get; } = area;
    public string Title { get; } = title;
    public string Status { get; set; } = "Pending";
    public double ElapsedMilliseconds { get; set; }
    public string? Error { get; set; }
    public Dictionary<string, double> Metrics { get; } = [];
}
