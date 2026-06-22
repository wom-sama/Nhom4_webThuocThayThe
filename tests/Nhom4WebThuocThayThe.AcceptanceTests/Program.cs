using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.RegularExpressions;

var repoRoot = FindRepoRoot();
var runtime = await WebAppRuntime.StartAsync(repoRoot);
var results = new List<TestResult>();

try
{
    var tests = AcceptanceTests.CreateAll(runtime);
    foreach (var test in tests)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await test.Execute();
            stopwatch.Stop();
            results.Add(new TestResult(test.Id, test.Area, test.Title, "Pass", stopwatch.ElapsedMilliseconds, null));
            Console.WriteLine($"PASS {test.Id} {test.Title} ({stopwatch.ElapsedMilliseconds} ms)");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            results.Add(new TestResult(test.Id, test.Area, test.Title, "Fail", stopwatch.ElapsedMilliseconds, ex.Message));
            Console.WriteLine($"FAIL {test.Id} {test.Title}: {ex.Message}");
        }
    }
}
finally
{
    await runtime.DisposeAsync();
}

var restartRuntime = await WebAppRuntime.StartAsync(repoRoot);
try
{
    var restartStopwatch = Stopwatch.StartNew();
    try
    {
        using var restartClient = restartRuntime.CreateClient();
        var persistedHtml = await restartClient.GetStringAsync("/Drugs?keyword=Persistence%20QA");
        restartStopwatch.Stop();
        if (!persistedHtml.Contains("Persistence QA 123mg"))
        {
            throw new InvalidOperationException("drug created before restart was not persisted");
        }

        results.Add(new TestResult(
            "TC30",
            "Persistence",
            "Created drug survives application restart",
            "Pass",
            restartStopwatch.ElapsedMilliseconds,
            null));
        Console.WriteLine($"PASS TC30 Created drug survives application restart ({restartStopwatch.ElapsedMilliseconds} ms)");
    }
    catch (Exception ex)
    {
        restartStopwatch.Stop();
        results.Add(new TestResult(
            "TC30",
            "Persistence",
            "Created drug survives application restart",
            "Fail",
            restartStopwatch.ElapsedMilliseconds,
            ex.Message));
        Console.WriteLine($"FAIL TC30 Created drug survives application restart: {ex.Message}");
    }

    var assetStopwatch = Stopwatch.StartNew();
    try
    {
        using var assetClient = restartRuntime.CreateClient();
        assetClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
        using var response = await assetClient.GetAsync("/css/site.css");
        var bytes = await response.Content.ReadAsByteArrayAsync();
        assetStopwatch.Stop();

        if (!response.IsSuccessStatusCode ||
            response.Content.Headers.ContentType?.MediaType != "text/css" ||
            bytes.Length < 1_000)
        {
            throw new InvalidOperationException("browser-like stylesheet response was empty or invalid");
        }

        assetClient.DefaultRequestHeaders.AcceptEncoding.Clear();
        var homeHtml = await assetClient.GetStringAsync("/");
        if (homeHtml.Contains("Nhom4WebThuocThayThe.styles.css"))
        {
            throw new InvalidOperationException("layout still references the removed scoped stylesheet");
        }

        results.Add(new TestResult(
            "TC31",
            "Static assets",
            "Browser-like CSS request returns complete stylesheet",
            "Pass",
            assetStopwatch.ElapsedMilliseconds,
            null));
        Console.WriteLine($"PASS TC31 Browser-like CSS request returns complete stylesheet ({assetStopwatch.ElapsedMilliseconds} ms)");
    }
    catch (Exception ex)
    {
        assetStopwatch.Stop();
        results.Add(new TestResult(
            "TC31",
            "Static assets",
            "Browser-like CSS request returns complete stylesheet",
            "Fail",
            assetStopwatch.ElapsedMilliseconds,
            ex.Message));
        Console.WriteLine($"FAIL TC31 Browser-like CSS request returns complete stylesheet: {ex.Message}");
    }

    var vendorAssetStopwatch = Stopwatch.StartNew();
    try
    {
        using var assetClient = restartRuntime.CreateClient();
        var requiredAssets = new[]
        {
            "/lib/bootstrap/dist/css/bootstrap.min.css",
            "/lib/bootstrap/dist/js/bootstrap.bundle.min.js",
            "/lib/jquery/dist/jquery.min.js",
            "/lib/jquery-validation/dist/jquery.validate.min.js",
            "/lib/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.min.js"
        };
        foreach (var path in requiredAssets)
        {
            using var response = await assetClient.GetAsync(path);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            if (!response.IsSuccessStatusCode || bytes.Length < 1_000)
            {
                throw new InvalidOperationException($"required vendor asset is missing or empty: {path}");
            }
        }

        vendorAssetStopwatch.Stop();
        results.Add(new TestResult(
            "TC32",
            "Static assets",
            "Bootstrap and validation vendor assets are published",
            "Pass",
            vendorAssetStopwatch.ElapsedMilliseconds,
            null));
        Console.WriteLine($"PASS TC32 Bootstrap and validation vendor assets are published ({vendorAssetStopwatch.ElapsedMilliseconds} ms)");
    }
    catch (Exception ex)
    {
        vendorAssetStopwatch.Stop();
        results.Add(new TestResult(
            "TC32",
            "Static assets",
            "Bootstrap and validation vendor assets are published",
            "Fail",
            vendorAssetStopwatch.ElapsedMilliseconds,
            ex.Message));
        Console.WriteLine($"FAIL TC32 Bootstrap and validation vendor assets are published: {ex.Message}");
    }

    var aiFallbackStopwatch = Stopwatch.StartNew();
    try
    {
        using var aiClient = restartRuntime.CreateClient();
        var details = await aiClient.GetStringAsync("/Drugs/Details/1");
        var tokenInput = Regex.Match(
            details,
            "<input[^>]*name=\"__RequestVerificationToken\"[^>]*>",
            RegexOptions.IgnoreCase);
        var tokenValue = Regex.Match(tokenInput.Value, "value=\"([^\"]+)\"", RegexOptions.IgnoreCase);
        if (!tokenInput.Success || !tokenValue.Success)
        {
            throw new InvalidOperationException("AI anti-forgery token is missing");
        }

        var token = WebUtility.HtmlDecode(tokenValue.Groups[1].Value);
        using var response = await aiClient.PostAsync(
            "/Drugs/ExplainAlternative",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["sourceId"] = "1",
                ["candidateId"] = "2",
                ["__RequestVerificationToken"] = token
            }));
        var json = await response.Content.ReadAsStringAsync();
        aiFallbackStopwatch.Stop();
        if (!response.IsSuccessStatusCode ||
            !json.Contains("Deterministic fallback") ||
            !json.Contains("\"isAiGenerated\":false") ||
            json.Contains("@nhom4.local", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("AI-disabled endpoint did not return a safe fallback");
        }

        results.Add(new TestResult(
            "TC33",
            "AI safety",
            "AI-disabled endpoint returns no-PII deterministic fallback",
            "Pass",
            aiFallbackStopwatch.ElapsedMilliseconds,
            null));
        Console.WriteLine($"PASS TC33 AI-disabled endpoint returns no-PII deterministic fallback ({aiFallbackStopwatch.ElapsedMilliseconds} ms)");
    }
    catch (Exception ex)
    {
        aiFallbackStopwatch.Stop();
        results.Add(new TestResult(
            "TC33",
            "AI safety",
            "AI-disabled endpoint returns no-PII deterministic fallback",
            "Fail",
            aiFallbackStopwatch.ElapsedMilliseconds,
            ex.Message));
        Console.WriteLine($"FAIL TC33 AI-disabled endpoint returns no-PII deterministic fallback: {ex.Message}");
    }

    var compressionStopwatch = Stopwatch.StartNew();
    try
    {
        using var compressionClient = restartRuntime.CreateClient();
        compressionClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
        using var response = await compressionClient.GetAsync("/css/site.css");
        var bytes = await response.Content.ReadAsByteArrayAsync();
        compressionStopwatch.Stop();
        var cacheControl = response.Headers.CacheControl;
        if (!response.IsSuccessStatusCode ||
            !response.Content.Headers.ContentEncoding.Contains("br") ||
            cacheControl?.Public != true ||
            cacheControl.MaxAge < TimeSpan.FromDays(7) ||
            bytes.Length >= 10_000)
        {
            throw new InvalidOperationException("static asset compression or seven-day cache policy is missing");
        }

        results.Add(new TestResult(
            "TC34",
            "Free hosting",
            "Static assets use Brotli and seven-day public cache",
            "Pass",
            compressionStopwatch.ElapsedMilliseconds,
            null));
        Console.WriteLine($"PASS TC34 Static assets use Brotli and seven-day public cache ({compressionStopwatch.ElapsedMilliseconds} ms)");
    }
    catch (Exception ex)
    {
        compressionStopwatch.Stop();
        results.Add(new TestResult(
            "TC34",
            "Free hosting",
            "Static assets use Brotli and seven-day public cache",
            "Fail",
            compressionStopwatch.ElapsedMilliseconds,
            ex.Message));
        Console.WriteLine($"FAIL TC34 Static assets use Brotli and seven-day public cache: {ex.Message}");
    }
}
finally
{
    await restartRuntime.DisposeAsync();
}

var reportDirectory = Path.Combine(repoRoot.FullName, "TestResults");
Directory.CreateDirectory(reportDirectory);
var reportPath = Path.Combine(reportDirectory, "acceptance-report.json");
await File.WriteAllTextAsync(reportPath, JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true }));

var failed = results.Count(item => item.Status == "Fail");
Console.WriteLine();
Console.WriteLine($"Acceptance summary: {results.Count - failed}/{results.Count} passed");
Console.WriteLine($"Report: {reportPath}");

return failed == 0 ? 0 : 1;

static DirectoryInfo FindRepoRoot()
{
    var current = new DirectoryInfo(AppContext.BaseDirectory);
    while (current is not null)
    {
        if (File.Exists(Path.Combine(current.FullName, "Nhom4WebThuocThayThe.csproj")))
        {
            return current;
        }

        current = current.Parent;
    }

    throw new InvalidOperationException("Cannot locate repository root.");
}

internal sealed record AcceptanceTest(string Id, string Area, string Title, Func<Task> Execute);

internal sealed record TestResult(string Id, string Area, string Title, string Status, long ElapsedMilliseconds, string? Error);

internal sealed class WebAppRuntime : IAsyncDisposable
{
    private readonly Process _process;

    private WebAppRuntime(DirectoryInfo repoRoot, Process process, Uri baseUri)
    {
        RepoRoot = repoRoot;
        _process = process;
        BaseUri = baseUri;
    }

    public DirectoryInfo RepoRoot { get; }

    public Uri BaseUri { get; }

    public long WorkingSetBytes => _process.HasExited ? 0 : _process.WorkingSet64;

    public static async Task<WebAppRuntime> StartAsync(DirectoryInfo repoRoot)
    {
        var port = GetFreePort();
        var baseUri = new Uri($"http://127.0.0.1:{port}");
        var projectPath = Path.Combine(repoRoot.FullName, "Nhom4WebThuocThayThe.csproj");
        var output = new List<string>();

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --no-build --no-launch-profile --configuration Release --project \"{projectPath}\" --urls {baseUri}",
                WorkingDirectory = repoRoot.FullName,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true
        };

        process.StartInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Testing";

        process.OutputDataReceived += (_, args) =>
        {
            if (args.Data is not null)
            {
                output.Add(args.Data);
            }
        };
        process.ErrorDataReceived += (_, args) =>
        {
            if (args.Data is not null)
            {
                output.Add(args.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        var runtime = new WebAppRuntime(repoRoot, process, baseUri);
        using var client = runtime.CreateClient();
        var deadline = DateTimeOffset.UtcNow.AddSeconds(30);
        while (DateTimeOffset.UtcNow < deadline)
        {
            if (process.HasExited)
            {
                throw new InvalidOperationException("Web app exited before readiness. " + string.Join(Environment.NewLine, output.TakeLast(20)));
            }

            try
            {
                using var response = await client.GetAsync("/");
                if (response.IsSuccessStatusCode)
                {
                    return runtime;
                }
            }
            catch
            {
                await Task.Delay(300);
            }
        }

        throw new TimeoutException("Web app did not become ready. " + string.Join(Environment.NewLine, output.TakeLast(20)));
    }

    public HttpClient CreateClient(bool allowAutoRedirect = true)
    {
        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = allowAutoRedirect,
            CookieContainer = new CookieContainer()
        };

        return new HttpClient(handler)
        {
            BaseAddress = BaseUri,
            Timeout = TimeSpan.FromSeconds(10)
        };
    }

    public async ValueTask DisposeAsync()
    {
        if (!_process.HasExited)
        {
            _process.Kill(entireProcessTree: true);
            await _process.WaitForExitAsync();
        }

        _process.Dispose();
    }

    private static int GetFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }
}

internal static class AcceptanceTests
{
    public static IReadOnlyCollection<AcceptanceTest> CreateAll(WebAppRuntime runtime)
    {
        return
        [
            new("TC01", "Smoke", "Home page renders app dashboard", async () =>
            {
                using var client = runtime.CreateClient();
                var html = await GetStringAsync(client, "/");
                Expect(html.Contains("Tìm thuốc và phương án thay thế"), "public home title missing");
                Expect(html.Contains("skip-link"), "skip link missing");
            }),
            new("TC02", "Search", "Search finds medicine by active ingredient", async () =>
            {
                using var client = runtime.CreateClient();
                var html = await GetStringAsync(client, "/Drugs?keyword=para");
                Expect(html.Contains("Paracetamol DHG 500mg"), "expected paracetamol result missing");
                Expect(html.Contains("trong kho"), "stock status missing from result card");
            }),
            new("TC03", "Search", "Search handles empty result state", async () =>
            {
                using var client = runtime.CreateClient();
                var html = await GetStringAsync(client, "/Drugs?keyword=zzzz-not-found");
                Expect(html.Contains("Không tìm thấy thuốc phù hợp"), "empty state missing");
            }),
            new("TC04", "Search", "Category filter narrows results", async () =>
            {
                using var client = runtime.CreateClient();
                var html = await GetStringAsync(client, "/Drugs?categoryId=2");
                Expect(html.Contains("Amoxicillin 500mg"), "antibiotic result missing");
                Expect(!html.Contains("Panadol 500mg"), "category filter leaked analgesic item");
            }),
            new("TC05", "Drug detail", "Drug detail shows same-active-ingredient alternatives", async () =>
            {
                using var client = runtime.CreateClient();
                var html = await GetStringAsync(client, "/Drugs/Details/1");
                Expect(html.Contains("Thuốc thay thế đề xuất"), "recommendation section missing");
                Expect(html.Contains("Paracetamol DHG 500mg"), "same-active-ingredient alternative missing");
            }),
            new("TC06", "Error handling", "Invalid drug detail returns clean 404", async () =>
            {
                using var client = runtime.CreateClient();
                using var response = await client.GetAsync("/Drugs/Details/9999");
                var body = await response.Content.ReadAsStringAsync();
                Expect(response.StatusCode == HttpStatusCode.NotFound, "expected 404");
                Expect(!body.Contains("Exception", StringComparison.OrdinalIgnoreCase), "404 leaked exception text");
            }),
            new("TC07", "RBAC", "Anonymous user is redirected from catalog admin", async () =>
            {
                using var client = runtime.CreateClient(allowAutoRedirect: false);
                using var response = await client.GetAsync("/DrugCatalog");
                Expect(response.StatusCode == HttpStatusCode.Redirect, "anonymous catalog access should redirect");
                Expect(response.Headers.Location?.ToString().Contains("/Auth/Login") == true, "redirect should target login");
            }),
            new("TC08", "Auth", "Invalid login returns validation error", async () =>
            {
                using var client = runtime.CreateClient();
                var html = await LoginAsync(client, "admin@nhom4.local", "WrongPassword", followRedirects: true);
                Expect(html.Contains("Email hoặc mật khẩu không đúng"), "invalid login error missing");
            }),
            new("TC09", "Auth", "Pharmacist login can open inventory", async () =>
            {
                using var client = runtime.CreateClient();
                await LoginAsync(client, "duocsi@nhom4.local", "Duocsi@123", followRedirects: true);
                var html = await GetStringAsync(client, "/Pharmacist/Inventory");
                Expect(html.Contains("Tồn kho và lô thuốc"), "inventory page missing");
            }),
            new("TC10", "RBAC", "Normal user is denied from catalog admin", async () =>
            {
                using var client = runtime.CreateClient(allowAutoRedirect: false);
                await LoginAsync(client, "user@nhom4.local", "User@123", followRedirects: false);
                using var response = await client.GetAsync("/Admin/DrugCatalog");
                Expect(response.StatusCode == HttpStatusCode.Redirect, "user should be redirected away from catalog");
                Expect(response.Headers.Location?.ToString().Contains("/Auth/AccessDenied") == true, "redirect should target access denied");
            }),
            new("TC11", "Session", "Logout clears protected access", async () =>
            {
                using var client = runtime.CreateClient(allowAutoRedirect: false);
                await LoginAsync(client, "admin@nhom4.local", "Admin@123", followRedirects: false);
                var home = await GetStringAsync(client, "/");
                var token = ExtractAntiforgeryToken(home);
                using var logout = await client.PostAsync("/Auth/Logout", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["__RequestVerificationToken"] = token
                }));
                Expect(logout.StatusCode == HttpStatusCode.Redirect, "logout should redirect");
                using var inventory = await client.GetAsync("/Inventory");
                Expect(inventory.StatusCode == HttpStatusCode.Redirect, "protected access should redirect after logout");
            }),
            new("TC12", "Catalog", "Catalog create validation remains on form", async () =>
            {
                using var client = runtime.CreateClient();
                await LoginAsync(client, "admin@nhom4.local", "Admin@123", followRedirects: true);
                var form = await GetStringAsync(client, "/Admin/DrugCatalog/Create");
                var token = ExtractAntiforgeryToken(form);
                var html = await PostFormStringAsync(client, "/Admin/DrugCatalog/Create", new Dictionary<string, string>
                {
                    ["__RequestVerificationToken"] = token,
                    ["Name"] = "",
                    ["Strength"] = "500mg",
                    ["Price"] = "1000",
                    ["CategoryId"] = "1",
                    ["DosageFormId"] = "1",
                    ["UnitId"] = "1",
                    ["ManufacturerId"] = "1",
                    ["ActiveIngredientId"] = "1",
                    ["ActiveIngredientStrength"] = "500mg",
                    ["IsActive"] = "true"
                });
                Expect(html.Contains("Thêm thuốc"), "create form did not stay open");
                Expect(html.Contains("field-validation-error"), "validation message missing");
            }),
            new("TC13", "Security", "Anti-forgery rejects inventory post without token", async () =>
            {
                using var client = runtime.CreateClient(allowAutoRedirect: false);
                await LoginAsync(client, "admin@nhom4.local", "Admin@123", followRedirects: false);
                using var response = await client.PostAsync("/Admin/Inventory/CreateBatch", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["DrugId"] = "2",
                    ["WarehouseId"] = "1",
                    ["BatchNumber"] = "NO-TOKEN",
                    ["Quantity"] = "1",
                    ["ImportedDate"] = "2026-05-26",
                    ["ExpiryDate"] = "2027-05-26"
                }));
                Expect(response.StatusCode == HttpStatusCode.BadRequest, "missing anti-forgery token should be rejected");
            }),
            new("TC14", "Cache", "Static stylesheet is served with revalidation metadata", async () =>
            {
                using var client = runtime.CreateClient();
                using var response = await client.GetAsync("/css/site.css");
                var css = await response.Content.ReadAsStringAsync();
                Expect(response.IsSuccessStatusCode, "site.css did not load");
                Expect(css.Contains(".app-header"), "stylesheet template class missing");
                Expect(response.Headers.ETag is not null || response.Content.Headers.LastModified is not null, "static asset lacks cache revalidation metadata");
            }),
            new("TC15", "Responsive", "CSS includes responsive collapse rules", async () =>
            {
                using var client = runtime.CreateClient();
                var css = await GetStringAsync(client, "/css/site.css");
                Expect(css.Contains("@media (max-width: 991.98px)"), "tablet breakpoint missing");
                Expect(css.Contains("grid-template-columns: 1fr"), "single-column collapse rule missing");
            }),
            new("TC16", "Accessibility", "Login form has labels and autocomplete", async () =>
            {
                using var client = runtime.CreateClient();
                var html = await GetStringAsync(client, "/Auth/Login");
                Expect(html.Contains("autocomplete=\"username\""), "username autocomplete missing");
                Expect(html.Contains("autocomplete=\"current-password\""), "password autocomplete missing");
                Expect(html.Contains("<label"), "form labels missing");
            }),
            new("TC17", "Performance", "Search endpoint responds within local threshold", async () =>
            {
                using var client = runtime.CreateClient();
                var timings = new List<long>();
                for (var i = 0; i < 20; i++)
                {
                    var stopwatch = Stopwatch.StartNew();
                    using var response = await client.GetAsync("/Drugs?keyword=para");
                    stopwatch.Stop();
                    Expect(response.IsSuccessStatusCode, "search request failed");
                    timings.Add(stopwatch.ElapsedMilliseconds);
                }

                Expect(timings.Average() < 650, $"average search time too high: {timings.Average():N0} ms");
                Expect(timings.Max() < 2000, $"max search time too high: {timings.Max()} ms");
            }),
            new("TC18", "Memory", "Repeated requests stay within memory budget", async () =>
            {
                using var client = runtime.CreateClient();
                for (var i = 0; i < 50; i++)
                {
                    using var response = await client.GetAsync(i % 2 == 0 ? "/" : "/Drugs?keyword=para");
                    Expect(response.IsSuccessStatusCode, "request failed during memory loop");
                }

                var megabytes = runtime.WorkingSetBytes / 1024 / 1024;
                Expect(megabytes < 350, $"working set too high: {megabytes} MB");
            }),
            new("TC19", "Debug", "Unknown route does not expose stack trace", async () =>
            {
                using var client = runtime.CreateClient();
                using var response = await client.GetAsync("/not-real-route");
                var body = await response.Content.ReadAsStringAsync();
                Expect(response.StatusCode == HttpStatusCode.NotFound, "unknown route should be 404");
                Expect(!body.Contains("StackTrace", StringComparison.OrdinalIgnoreCase), "stack trace leaked");
            }),
            new("TC20", "Concurrency", "Parallel public reads remain stable", async () =>
            {
                using var client = runtime.CreateClient();
                var requests = Enumerable.Range(0, 12)
                    .Select(index => client.GetAsync(index % 2 == 0 ? "/Drugs?keyword=para" : "/Drugs/Details/2"))
                    .ToArray();
                var responses = await Task.WhenAll(requests);
                try
                {
                    Expect(responses.All(item => item.IsSuccessStatusCode), "one or more parallel requests failed");
                }
                finally
                {
                    foreach (var response in responses)
                    {
                        response.Dispose();
                    }
                }
            }),
            new("TC21", "Recommendation", "Out-of-stock drug shows ranked recommendations", async () =>
            {
                using var client = runtime.CreateClient();
                var html = await GetStringAsync(client, "/Drugs/Details/1");
                Expect(html.Contains("Thuốc thay thế đề xuất"), "recommendation section missing");
                Expect(html.Contains("Paracetamol DHG 500mg"), "primary substitute missing");
                Expect(html.Contains("Rất phù hợp") || html.Contains("Phù hợp"), "recommendation score missing");
                Expect(html.Contains("Cùng hoạt chất"), "recommendation reasons missing");
            }),
            new("TC22", "Safety", "Signed-in safety profile produces allergy warning", async () =>
            {
                using var client = runtime.CreateClient();
                await LoginAsync(client, "duocsi@nhom4.local", "Duocsi@123", followRedirects: true);
                var html = await GetStringAsync(client, "/Drugs/Details/1");
                Expect(html.Contains("Hồ sơ an toàn"), "safety profile context missing");
                Expect(html.Contains("Dị ứng hoạt chất"), "allergy warning missing");
            }),
            new("TC23", "Expert review", "Expert can open recommendation review workflow", async () =>
            {
                using var client = runtime.CreateClient();
                await LoginAsync(client, "chuyengia@nhom4.local", "Chuyengia@123", followRedirects: true);
                var html = await GetStringAsync(client, "/Expert/Reviews");
                Expect(html.Contains("Đánh giá đề xuất thuốc thay thế"), "expert review page missing");
                Expect(html.Contains("Panadol 500mg"), "review source drug missing");
            }),
            new("TC24", "RBAC", "Normal user cannot open reports dashboard", async () =>
            {
                using var client = runtime.CreateClient(allowAutoRedirect: false);
                await LoginAsync(client, "user@nhom4.local", "User@123", followRedirects: false);
                using var response = await client.GetAsync("/Admin/Reports");
                Expect(response.StatusCode == HttpStatusCode.Redirect, "normal user should be redirected from reports");
                Expect(response.Headers.Location?.ToString().Contains("/Auth/AccessDenied") == true, "reports denial should target access denied");
            }),
            new("TC25", "Reporting", "Admin dashboard exposes stock and audit metrics", async () =>
            {
                using var client = runtime.CreateClient();
                await LoginAsync(client, "admin@nhom4.local", "Admin@123", followRedirects: true);
                var html = await GetStringAsync(client, "/Admin/Reports");
                Expect(html.Contains("Tổng quan hệ thống thuốc thay thế"), "dashboard title missing");
                Expect(html.Contains("Rủi ro tồn kho"), "stock risk table missing");
                Expect(html.Contains("Nhật ký kiểm toán gần đây"), "audit log section missing");
            }),
            new("TC26", "External data", "Admin can inspect external data registry", async () =>
            {
                using var client = runtime.CreateClient();
                await LoginAsync(client, "admin@nhom4.local", "Admin@123", followRedirects: true);
                var html = await GetStringAsync(client, "/Admin/ExternalData");
                Expect(html.Contains("DrugBank"), "DrugBank source missing");
                Expect(html.Contains("PubChem"), "PubChem source missing");
                Expect(html.Contains("Đánh dấu đã đồng bộ"), "sync action missing");
            }),
            new("TC27", "Backup", "Admin can download backup metadata", async () =>
            {
                using var client = runtime.CreateClient();
                await LoginAsync(client, "admin@nhom4.local", "Admin@123", followRedirects: true);
                var dashboard = await GetStringAsync(client, "/Admin/Reports");
                var token = ExtractAntiforgeryToken(dashboard);
                using var response = await client.PostAsync("/Admin/Reports/DownloadBackup", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["__RequestVerificationToken"] = token
                }));
                var json = await response.Content.ReadAsStringAsync();
                Expect(response.IsSuccessStatusCode, "backup download failed");
                Expect(response.Content.Headers.ContentType?.MediaType == "application/json", "backup should be JSON");
                Expect(json.Contains("DrugCount"), "backup metadata missing drug count");
            }),
            new("TC28", "Expert review", "Expert review update creates visible audit evidence", async () =>
            {
                using var client = runtime.CreateClient();
                await LoginAsync(client, "chuyengia@nhom4.local", "Chuyengia@123", followRedirects: true);
                var reviewPage = await GetStringAsync(client, "/Expert/Reviews");
                var token = ExtractAntiforgeryToken(reviewPage);
                var updated = await PostFormStringAsync(client, "/Expert/Reviews/Update", new Dictionary<string, string>
                {
                    ["__RequestVerificationToken"] = token,
                    ["id"] = "1",
                    ["status"] = "Chap nhan",
                    ["note"] = "Da kiem tra cung hoat chat va ham luong."
                });
                Expect(updated.Contains("Đã cập nhật đánh giá chuyên gia"), "review success evidence missing");
                Expect(updated.Contains("Chấp nhận"), "updated review status missing");
            }),
            new("TC29", "Persistence", "Admin creates a drug in SQL Server", async () =>
            {
                using var client = runtime.CreateClient();
                var existing = await GetStringAsync(client, "/Drugs?keyword=Persistence%20QA");
                if (existing.Contains("Persistence QA 123mg"))
                {
                    return;
                }

                await LoginAsync(client, "admin@nhom4.local", "Admin@123", followRedirects: true);
                var form = await GetStringAsync(client, "/Admin/DrugCatalog/Create");
                var token = ExtractAntiforgeryToken(form);
                var catalog = await PostFormStringAsync(client, "/Admin/DrugCatalog/Create", new Dictionary<string, string>
                {
                    ["__RequestVerificationToken"] = token,
                    ["Name"] = "Persistence QA 123mg",
                    ["Strength"] = "123mg",
                    ["Price"] = "1234",
                    ["CategoryId"] = "1",
                    ["DosageFormId"] = "1",
                    ["UnitId"] = "1",
                    ["ManufacturerId"] = "1",
                    ["ActiveIngredientId"] = "1",
                    ["ActiveIngredientStrength"] = "123mg",
                    ["IsActive"] = "true",
                    ["Description"] = "Integration test record for SQL persistence.",
                    ["Usage"] = "Test only.",
                    ["Contraindications"] = "Test only."
                });
                Expect(catalog.Contains("Persistence QA 123mg"), "created drug missing from catalog");
            }),
            new("TC35", "Authentication UI", "Login page never renders demo credentials or role selector", async () =>
            {
                using var client = runtime.CreateClient();
                var html = await GetStringAsync(client, "/Auth/Login");
                Expect(!html.Contains("@nhom4.local", StringComparison.OrdinalIgnoreCase), "login leaked a demo email");
                Expect(!html.Contains("Admin@123", StringComparison.Ordinal), "login leaked a demo password");
                Expect(!html.Contains("Duocsi@123", StringComparison.Ordinal), "login leaked a demo password");
                Expect(!html.Contains("Chuyengia@123", StringComparison.Ordinal), "login leaked a demo password");
                Expect(!html.Contains("User@123", StringComparison.Ordinal), "login leaked a demo password");
                Expect(!html.Contains("<select", StringComparison.OrdinalIgnoreCase), "login must not allow role selection");
                Expect(!html.Contains("auth-benefits", StringComparison.OrdinalIgnoreCase), "login still renders verbose benefit bullets");
            }),
            new("TC36", "Localization", "Vietnamese culture is applied to HTML and response headers", async () =>
            {
                using var client = runtime.CreateClient();
                using var response = await client.GetAsync("/");
                var html = await response.Content.ReadAsStringAsync();
                Expect(response.IsSuccessStatusCode, "home request failed");
                Expect(html.Contains("<html lang=\"vi\"", StringComparison.OrdinalIgnoreCase), "html language is not vi");
                Expect(response.Content.Headers.ContentLanguage.Contains("vi-VN"), "Content-Language is not vi-VN");
            }),
            new("TC37", "Role routing", "Every role Area requires authentication", async () =>
            {
                using var client = runtime.CreateClient(allowAutoRedirect: false);
                var areas = new[] { "/Admin", "/Pharmacist", "/Expert", "/User" };
                foreach (var area in areas)
                {
                    using var response = await client.GetAsync(area);
                    Expect(response.StatusCode == HttpStatusCode.Redirect, $"anonymous request unexpectedly opened {area}");
                    Expect(response.Headers.Location?.ToString().Contains("/Auth/Login", StringComparison.OrdinalIgnoreCase) == true,
                        $"{area} did not redirect anonymous request to login");
                }
            }),
            new("TC38", "Role isolation", "Four role Areas enforce all sixteen access directions", async () =>
            {
                var roles = new[]
                {
                    ("admin@nhom4.local", "Admin@123", "/Admin"),
                    ("duocsi@nhom4.local", "Duocsi@123", "/Pharmacist"),
                    ("chuyengia@nhom4.local", "Chuyengia@123", "/Expert"),
                    ("user@nhom4.local", "User@123", "/User")
                };

                foreach (var current in roles)
                {
                    using var client = runtime.CreateClient(allowAutoRedirect: false);
                    using (var login = await SubmitLoginAsync(client, current.Item1, current.Item2))
                    {
                        Expect(IsRedirect(login.StatusCode), $"{current.Item1} login failed");
                    }

                    foreach (var target in roles)
                    {
                        using var response = await client.GetAsync(target.Item3);
                        if (string.Equals(current.Item3, target.Item3, StringComparison.OrdinalIgnoreCase))
                        {
                            Expect(response.IsSuccessStatusCode, $"{current.Item1} cannot open own Area {target.Item3}");
                            var ownAreaHtml = await response.Content.ReadAsStringAsync();
                            Expect(ownAreaHtml.Contains("quick-action-grid"), $"quick actions missing for {target.Item3}");
                        }
                        else
                        {
                            Expect(response.StatusCode == HttpStatusCode.Redirect,
                                $"{current.Item1} unexpectedly opened {target.Item3}");
                            Expect(response.Headers.Location?.ToString().Contains("/Auth/AccessDenied", StringComparison.OrdinalIgnoreCase) == true,
                                $"cross-role denial for {target.Item3} did not use access denied");
                        }
                    }
                }
            }),
            new("TC39", "Navigation isolation", "Public and role shells expose only relevant navigation", async () =>
            {
                using var publicClient = runtime.CreateClient();
                var publicHtml = await GetStringAsync(publicClient, "/");
                Expect(!publicHtml.Contains("href=\"/Admin", StringComparison.OrdinalIgnoreCase), "public shell leaked Admin navigation");
                Expect(!publicHtml.Contains("href=\"/Pharmacist", StringComparison.OrdinalIgnoreCase), "public shell leaked Pharmacist navigation");
                Expect(!publicHtml.Contains("href=\"/Expert", StringComparison.OrdinalIgnoreCase), "public shell leaked Expert navigation");

                using var userClient = runtime.CreateClient();
                await LoginAsync(userClient, "user@nhom4.local", "User@123", followRedirects: true);
                var userHtml = await GetStringAsync(userClient, "/User");
                Expect(userHtml.Contains("Lịch sử tra cứu"), "User navigation missing history");
                Expect(!userHtml.Contains("href=\"/Admin", StringComparison.OrdinalIgnoreCase), "User shell leaked Admin navigation");
                Expect(!userHtml.Contains("Kho rủi ro"), "User shell leaked Pharmacist navigation");
            }),
            new("TC40", "AI Area route", "Pharmacist detail binds AI request to its Area endpoint", async () =>
            {
                using var client = runtime.CreateClient();
                await LoginAsync(client, "duocsi@nhom4.local", "Duocsi@123", followRedirects: true);
                var html = await GetStringAsync(client, "/Pharmacist/Workspace/Details/1");
                Expect(html.Contains("data-endpoint=\"/Pharmacist/Workspace/ExplainAlternative\""), "AI button did not bind Area endpoint");
                Expect(html.Contains("Giải thích bằng AI (tùy chọn)"), "AI action label is not localized");
            }),
            new("TC41", "Home usability", "Home category filter is functional and submitted", async () =>
            {
                using var client = runtime.CreateClient();
                var html = await GetStringAsync(client, "/");
                Expect(html.Contains("id=\"homeCategory\""), "home category control missing");
                Expect(html.Contains("name=\"categoryId\""), "home category is not submitted");
                Expect(!Regex.IsMatch(html, "<select[^>]*id=\"homeCategory\"[^>]*disabled", RegexOptions.IgnoreCase),
                    "home category still appears interactive but disabled");
                Expect(html.Contains("Tất cả nhóm thuốc"), "category all-option missing");
                Expect(html.Contains("metric-strip"), "compact home metric strip missing");
                Expect(!html.Contains("workflow-card"), "home still renders verbose workflow cards");
            }),
            new("TC42", "Decision-first detail", "Availability and alternatives precede pharmacology detail", async () =>
            {
                using var client = runtime.CreateClient();
                var html = await GetStringAsync(client, "/Drugs/Details/1");
                var decisionIndex = html.IndexOf("decision-summary", StringComparison.Ordinal);
                var pharmacologyIndex = html.IndexOf("detail-information", StringComparison.Ordinal);
                Expect(decisionIndex >= 0, "decision summary missing");
                Expect(pharmacologyIndex > decisionIndex, "pharmacology appears before the primary decision");
                Expect(html.Contains("recommendation-signals"), "top decision signals missing");
                Expect(html.Contains("reason-disclosure"), "progressive reason disclosure missing");
            }),
            new("TC43", "Role quick actions", "Every role dashboard defines task-oriented quick actions", async () =>
            {
                var viewPaths = new[]
                {
                    Path.Combine("Areas", "Admin", "Views", "Home", "Index.cshtml"),
                    Path.Combine("Areas", "Pharmacist", "Views", "Home", "Index.cshtml"),
                    Path.Combine("Areas", "Expert", "Views", "Home", "Index.cshtml"),
                    Path.Combine("Areas", "User", "Views", "Home", "Index.cshtml")
                };
                foreach (var relativePath in viewPaths)
                {
                    var source = await File.ReadAllTextAsync(Path.Combine(runtime.RepoRoot.FullName, relativePath));
                    Expect(source.Contains("quick-action-grid"), $"quick actions missing in {relativePath}");
                    Expect(source.Contains("data-lucide="), $"Lucide contract missing in {relativePath}");
                }
            }),
            new("TC44", "Icon runtime", "Self-hosted Lucide asset is published and referenced", async () =>
            {
                using var client = runtime.CreateClient();
                var home = await GetStringAsync(client, "/");
                Expect(home.Contains("/lib/lucide/lucide.min.js"), "Lucide runtime is not referenced");
                using var response = await client.GetAsync("/lib/lucide/lucide.min.js");
                var bytes = await response.Content.ReadAsByteArrayAsync();
                Expect(response.IsSuccessStatusCode, "Lucide runtime was not served");
                Expect(bytes.Length > 100_000, "Lucide runtime appears incomplete");
            })
        ];
    }

    private static async Task<string> LoginAsync(HttpClient client, string email, string password, bool followRedirects)
    {
        using var response = await SubmitLoginAsync(client, email, password);

        if (!followRedirects || !IsRedirect(response.StatusCode))
        {
            return WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
        }

        var location = response.Headers.Location?.ToString() ?? "/";
        return await GetStringAsync(client, location);
    }

    private static async Task<HttpResponseMessage> SubmitLoginAsync(HttpClient client, string email, string password)
    {
        var loginHtml = await GetStringAsync(client, "/Auth/Login");
        var token = ExtractAntiforgeryToken(loginHtml);
        return await client.PostAsync("/Auth/Login", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Email"] = email,
            ["Password"] = password,
            ["ReturnUrl"] = "",
            ["__RequestVerificationToken"] = token
        }));
    }

    private static async Task<string> GetStringAsync(HttpClient client, string path)
    {
        using var response = await client.GetAsync(path);
        var body = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
        Expect(response.IsSuccessStatusCode, $"{path} returned {(int)response.StatusCode}");
        return body;
    }

    private static async Task<string> PostFormStringAsync(HttpClient client, string path, Dictionary<string, string> form)
    {
        using var response = await client.PostAsync(path, new FormUrlEncodedContent(form));
        var body = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
        Expect(response.IsSuccessStatusCode, $"{path} returned {(int)response.StatusCode}");
        return body;
    }

    private static string ExtractAntiforgeryToken(string html)
    {
        var input = Regex.Match(html, "<input[^>]*name=\"__RequestVerificationToken\"[^>]*>", RegexOptions.IgnoreCase);
        Expect(input.Success, "anti-forgery token input missing");
        var value = Regex.Match(input.Value, "value=\"([^\"]+)\"", RegexOptions.IgnoreCase);
        Expect(value.Success, "anti-forgery token value missing");
        return WebUtility.HtmlDecode(value.Groups[1].Value);
    }

    private static bool IsRedirect(HttpStatusCode statusCode)
    {
        return statusCode is HttpStatusCode.Redirect
            or HttpStatusCode.Moved
            or HttpStatusCode.RedirectMethod
            or HttpStatusCode.TemporaryRedirect
            or HttpStatusCode.PermanentRedirect;
    }

    private static void Expect(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
