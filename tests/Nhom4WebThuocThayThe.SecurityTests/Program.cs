using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.RegularExpressions;

var repoRoot = FindRepoRoot();
var runtime = await WebAppRuntime.StartAsync(repoRoot);
var results = new List<TestResult>();

try
{
    foreach (var test in SecurityTests.CreateAll(runtime))
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

var reportDirectory = Path.Combine(repoRoot.FullName, "TestResults");
Directory.CreateDirectory(reportDirectory);
var reportPath = Path.Combine(reportDirectory, "security-report.json");
await File.WriteAllTextAsync(reportPath, JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true }));

var failed = results.Count(item => item.Status == "Fail");
Console.WriteLine();
Console.WriteLine($"Security summary: {results.Count - failed}/{results.Count} passed");
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

internal sealed record SecurityTest(string Id, string Area, string Title, Func<Task> Execute);

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
                Arguments = $"run --no-build --configuration Release --project \"{projectPath}\" --urls {baseUri}",
                WorkingDirectory = repoRoot.FullName,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true
        };

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
        return new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = allowAutoRedirect,
            CookieContainer = new CookieContainer()
        })
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

internal static class SecurityTests
{
    public static IReadOnlyCollection<SecurityTest> CreateAll(WebAppRuntime runtime)
    {
        return
        [
            new("SEC01", "Password storage", "Seed users use PBKDF2 hash storage", async () =>
            {
                var appUser = await File.ReadAllTextAsync(Path.Combine(runtime.RepoRoot.FullName, "Models", "AppUser.cs"));
                var accountService = await File.ReadAllTextAsync(Path.Combine(runtime.RepoRoot.FullName, "Services", "InMemoryUserAccountService.cs"));

                Expect(appUser.Contains("PasswordHash"), "AppUser should expose PasswordHash");
                Expect(appUser.Contains("PasswordSalt"), "AppUser should expose PasswordSalt");
                Expect(!Regex.IsMatch(appUser, @"required\\s+string\\s+Password\\s*\\{"), "AppUser still exposes plaintext Password");
                Expect(!accountService.Contains("Admin@123"), "admin plaintext password stored in account service");
                Expect(accountService.Contains("Rfc2898DeriveBytes.Pbkdf2"), "PBKDF2 verification missing");
            }),
            new("SEC02", "Auth", "Valid hashed password still authenticates", async () =>
            {
                using var client = runtime.CreateClient();
                var html = await LoginAsync(client, "admin@nhom4.local", "Admin@123", followRedirects: true);
                Expect(html.Contains("Tổng quan vận hành"), "login with hashed credentials failed");
            }),
            new("SEC03", "Auth", "Invalid login does not set auth cookie", async () =>
            {
                using var client = runtime.CreateClient(allowAutoRedirect: false);
                var login = await GetStringAsync(client, "/Auth/Login");
                var token = ExtractAntiforgeryToken(login);
                using var response = await client.PostAsync("/Auth/Login", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["Email"] = "admin@nhom4.local",
                    ["Password"] = "WrongPassword",
                    ["ReturnUrl"] = "",
                    ["__RequestVerificationToken"] = token
                }));

                var cookies = response.Headers.TryGetValues("Set-Cookie", out var values) ? string.Join(";", values) : string.Empty;
                Expect(!cookies.Contains(".AspNetCore.Cookies"), "invalid login set authentication cookie");
            }),
            new("SEC04", "Cookie", "Authentication cookie is HttpOnly and SameSite", async () =>
            {
                using var client = runtime.CreateClient(allowAutoRedirect: false);
                var login = await GetStringAsync(client, "/Auth/Login");
                var token = ExtractAntiforgeryToken(login);
                using var response = await client.PostAsync("/Auth/Login", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["Email"] = "duocsi@nhom4.local",
                    ["Password"] = "Duocsi@123",
                    ["ReturnUrl"] = "",
                    ["__RequestVerificationToken"] = token
                }));

                var cookie = response.Headers.GetValues("Set-Cookie").First(item => item.Contains(".AspNetCore.Cookies"));
                Expect(cookie.Contains("httponly", StringComparison.OrdinalIgnoreCase), "auth cookie missing HttpOnly");
                Expect(cookie.Contains("samesite=lax", StringComparison.OrdinalIgnoreCase), "auth cookie missing SameSite=Lax");
            }),
            new("SEC05", "RBAC", "Role access matrix is enforced", async () =>
            {
                using var pharmacist = runtime.CreateClient(allowAutoRedirect: false);
                await LoginAsync(pharmacist, "duocsi@nhom4.local", "Duocsi@123", followRedirects: false);
                using var pharmacistInventory = await pharmacist.GetAsync("/Inventory");
                Expect(pharmacistInventory.IsSuccessStatusCode, "pharmacist should access inventory");

                using var normalUser = runtime.CreateClient(allowAutoRedirect: false);
                await LoginAsync(normalUser, "user@nhom4.local", "User@123", followRedirects: false);
                using var userInventory = await normalUser.GetAsync("/Inventory");
                Expect(userInventory.StatusCode == HttpStatusCode.Redirect, "normal user should not access inventory");
                Expect(userInventory.Headers.Location?.ToString().Contains("/Auth/AccessDenied") == true, "normal user should redirect to access denied");
            }),
            new("SEC06", "CSRF", "Protected POST without anti-forgery token is rejected", async () =>
            {
                using var client = runtime.CreateClient(allowAutoRedirect: false);
                await LoginAsync(client, "admin@nhom4.local", "Admin@123", followRedirects: false);
                using var response = await client.PostAsync("/DrugCatalog/Create", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["Name"] = "Tamper",
                    ["Strength"] = "1mg",
                    ["Price"] = "1"
                }));

                Expect(response.StatusCode == HttpStatusCode.BadRequest, "missing CSRF token should return 400");
            }),
            new("SEC07", "Open redirect", "External returnUrl is ignored after login", async () =>
            {
                using var client = runtime.CreateClient(allowAutoRedirect: false);
                var login = await GetStringAsync(client, "/Auth/Login?returnUrl=https%3A%2F%2Fevil.example%2F");
                var token = ExtractAntiforgeryToken(login);
                using var response = await client.PostAsync("/Auth/Login", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["Email"] = "admin@nhom4.local",
                    ["Password"] = "Admin@123",
                    ["ReturnUrl"] = "https://evil.example/",
                    ["__RequestVerificationToken"] = token
                }));

                Expect(response.StatusCode == HttpStatusCode.Redirect, "login should redirect locally");
                Expect(response.Headers.Location?.ToString() != "https://evil.example/", "external returnUrl was accepted");
            }),
            new("SEC08", "XSS", "Search keyword is HTML encoded", async () =>
            {
                using var client = runtime.CreateClient();
                var payload = WebUtility.UrlEncode("<script>alert(1)</script>");
                var html = await GetStringAsync(client, "/Drugs?keyword=" + payload);
                Expect(!html.Contains("<script>alert(1)</script>", StringComparison.OrdinalIgnoreCase), "raw script payload rendered");
                Expect(html.Contains("&lt;script&gt;alert(1)&lt;/script&gt;") || html.Contains("Khong tim thay"), "payload was not safely handled");
            }),
            new("SEC09", "Error leakage", "Bad routes do not leak internals", async () =>
            {
                using var client = runtime.CreateClient();
                using var response = await client.GetAsync("/Drugs/Details/999999");
                var body = await response.Content.ReadAsStringAsync();
                Expect(response.StatusCode == HttpStatusCode.NotFound, "missing drug should return 404");
                Expect(!body.Contains("StackTrace", StringComparison.OrdinalIgnoreCase), "stack trace leaked");
                Expect(!body.Contains("PasswordHash", StringComparison.OrdinalIgnoreCase), "sensitive model detail leaked");
                Expect(!body.Contains(runtime.RepoRoot.FullName, StringComparison.OrdinalIgnoreCase), "local path leaked");
            }),
            new("SEC10", "Anti-forgery", "Sensitive forms render a non-empty anti-forgery token", async () =>
            {
                using var client = runtime.CreateClient();
                await LoginAsync(client, "admin@nhom4.local", "Admin@123", followRedirects: true);
                using var response = await client.GetAsync("/Inventory");
                Expect(response.IsSuccessStatusCode, "inventory should load for admin");
                var body = await response.Content.ReadAsStringAsync();
                Expect(!body.Contains("__RequestVerificationToken\" value=\"\"", StringComparison.OrdinalIgnoreCase), "blank anti-forgery token rendered");
            }),
            new("SEC11", "Brute force", "Login endpoint rate limits repeated attempts", async () =>
            {
                using var client = runtime.CreateClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("N4WTT-Security-Rate-Limit-Probe/1.0");
                var throttled = false;
                for (var attempt = 0; attempt < 22; attempt++)
                {
                    var login = await GetStringAsync(client, "/Auth/Login");
                    var token = ExtractAntiforgeryToken(login);
                    using var response = await client.PostAsync("/Auth/Login", new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["Email"] = "admin@nhom4.local",
                        ["Password"] = $"Wrong-{attempt}",
                        ["ReturnUrl"] = "",
                        ["__RequestVerificationToken"] = token
                    }));
                    throttled = throttled || response.StatusCode == HttpStatusCode.TooManyRequests;
                }

                Expect(throttled, "repeated login attempts were not rate limited");
            }),
            new("SEC12", "Security headers", "Dynamic responses include browser hardening headers", async () =>
            {
                using var client = runtime.CreateClient();
                using var response = await client.GetAsync("/");
                response.EnsureSuccessStatusCode();
                Expect(response.Headers.TryGetValues("X-Content-Type-Options", out var contentTypes) && contentTypes.Contains("nosniff"), "X-Content-Type-Options missing");
                Expect(response.Headers.TryGetValues("X-Frame-Options", out var frames) && frames.Contains("DENY"), "X-Frame-Options missing");
                Expect(response.Headers.TryGetValues("Content-Security-Policy", out var policies) && policies.Any(value => value.Contains("frame-ancestors 'none'")), "Content-Security-Policy missing");
                Expect(response.Headers.TryGetValues("Referrer-Policy", out var referrers) && referrers.Contains("no-referrer"), "Referrer-Policy missing");
            }),
            new("SEC13", "AI method", "AI explanation rejects GET requests", async () =>
            {
                using var client = runtime.CreateClient(allowAutoRedirect: false);
                using var response = await client.GetAsync("/Drugs/ExplainAlternative?sourceId=1&candidateId=2");
                Expect(response.StatusCode == HttpStatusCode.MethodNotAllowed, "AI endpoint should be POST-only");
            }),
            new("SEC14", "AI CSRF", "AI explanation rejects POST without anti-forgery token", async () =>
            {
                using var client = runtime.CreateClient(allowAutoRedirect: false);
                using var response = await client.PostAsync(
                    "/Drugs/ExplainAlternative",
                    new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["sourceId"] = "1",
                        ["candidateId"] = "2"
                    }));
                Expect(response.StatusCode == HttpStatusCode.BadRequest, "AI POST without CSRF token should return 400");
            }),
            new("SEC15", "AI privacy", "AI fallback response excludes PII and executable markup", async () =>
            {
                using var client = runtime.CreateClient();
                var details = await GetStringAsync(client, "/Drugs/Details/1");
                var token = ExtractAntiforgeryToken(details);
                using var response = await client.PostAsync(
                    "/Drugs/ExplainAlternative",
                    new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["sourceId"] = "1",
                        ["candidateId"] = "2",
                        ["__RequestVerificationToken"] = token
                    }));
                var body = await response.Content.ReadAsStringAsync();
                Expect(response.IsSuccessStatusCode, "AI fallback request failed");
                Expect(!body.Contains("@nhom4.local", StringComparison.OrdinalIgnoreCase), "AI response leaked an account email");
                Expect(!body.Contains("<script", StringComparison.OrdinalIgnoreCase), "AI response contains executable markup");
                Expect(body.Contains("Deterministic fallback"), "disabled AI did not use deterministic fallback");
            }),
            new("SEC16", "AI secret", "API key is absent from browser-delivered content", async () =>
            {
                using var client = runtime.CreateClient();
                var html = await GetStringAsync(client, "/Drugs/Details/1");
                var javascript = await GetStringAsync(client, "/js/site.js");
                var delivered = html + javascript;
                Expect(!delivered.Contains("x-goog-api-key", StringComparison.OrdinalIgnoreCase), "API authentication header leaked to browser content");
                Expect(!delivered.Contains("GEMINI_API_KEY", StringComparison.OrdinalIgnoreCase), "API key environment name leaked to browser content");
                Expect(!Regex.IsMatch(delivered, @"AIza[0-9A-Za-z_-]{20,}"), "Google API key pattern leaked to browser content");
            }),
            new("SEC17", "AI abuse", "AI explanation rate limits repeated requests", async () =>
            {
                using var client = runtime.CreateClient();
                var details = await GetStringAsync(client, "/Drugs/Details/1");
                var token = ExtractAntiforgeryToken(details);
                var throttled = false;
                for (var attempt = 0; attempt < 8; attempt++)
                {
                    using var response = await client.PostAsync(
                        "/Drugs/ExplainAlternative",
                        new FormUrlEncodedContent(new Dictionary<string, string>
                        {
                            ["sourceId"] = "1",
                            ["candidateId"] = "2",
                            ["__RequestVerificationToken"] = token
                        }));
                    throttled = throttled || response.StatusCode == HttpStatusCode.TooManyRequests;
                }

                Expect(throttled, "repeated AI requests were not rate limited");
            })
        ];
    }

    private static async Task<string> LoginAsync(HttpClient client, string email, string password, bool followRedirects)
    {
        var loginHtml = await GetStringAsync(client, "/Auth/Login");
        var token = ExtractAntiforgeryToken(loginHtml);
        var response = await client.PostAsync("/Auth/Login", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Email"] = email,
            ["Password"] = password,
            ["ReturnUrl"] = "",
            ["__RequestVerificationToken"] = token
        }));

        if (!followRedirects || !IsRedirect(response.StatusCode))
        {
            return await response.Content.ReadAsStringAsync();
        }

        return await GetStringAsync(client, response.Headers.Location?.ToString() ?? "/");
    }

    private static async Task<string> GetStringAsync(HttpClient client, string path)
    {
        using var response = await client.GetAsync(path);
        var body = await response.Content.ReadAsStringAsync();
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
