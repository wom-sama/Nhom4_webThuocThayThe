using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.RegularExpressions;

var repoRoot = FindRepoRoot();
var runtime = await WebAppRuntime.StartAsync(repoRoot);
var results = new List<PerfResult>();

try
{
    using var publicClient = runtime.CreateClient();
    await WarmUpAsync(publicClient);

    results.Add(await MeasureRouteAsync(publicClient, "PERF01", "Public route latency", "/", iterations: 40, p95LimitMs: 450));
    results.Add(await MeasureRouteAsync(publicClient, "PERF02", "Drug search latency", "/Drugs?keyword=para", iterations: 50, p95LimitMs: 550));
    results.Add(await MeasureRouteAsync(publicClient, "PERF03", "Drug detail latency", "/Drugs/Details/1", iterations: 40, p95LimitMs: 500));
    results.Add(await MeasureStaticAssetAsync(runtime));
    results.Add(await MeasureAuthenticatedInventoryAsync(runtime));
    results.Add(await MeasureConcurrencyBurstAsync(runtime));
    results.Add(await MeasureSustainedWindowAsync(runtime));
    results.Add(await MeasureMemorySustainAsync(runtime));
    results.Add(await MeasureHundredVirtualUsersAsync(runtime));
    results.Add(await MeasureSomeeFreeProfileAsync(runtime));
}
finally
{
    await runtime.DisposeAsync();
}

var reportDirectory = Path.Combine(repoRoot.FullName, "TestResults");
Directory.CreateDirectory(reportDirectory);
var reportPath = Path.Combine(reportDirectory, "performance-report.json");
await File.WriteAllTextAsync(reportPath, JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true }));

foreach (var result in results)
{
    Console.WriteLine($"{result.Status} {result.Id} {result.Name}: p50={result.P50Ms:N0}ms p95={result.P95Ms:N0}ms max={result.MaxMs:N0}ms rps={result.RequestsPerSecond:N1} {result.Message}");
}

var failed = results.Count(item => item.Status == "Fail");
Console.WriteLine();
Console.WriteLine($"Performance summary: {results.Count - failed}/{results.Count} passed");
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

static async Task WarmUpAsync(HttpClient client)
{
    foreach (var route in new[] { "/", "/Drugs?keyword=para", "/Drugs/Details/1", "/css/site.css" })
    {
        using var response = await client.GetAsync(route);
        response.EnsureSuccessStatusCode();
    }
}

static async Task<PerfResult> MeasureRouteAsync(HttpClient client, string id, string name, string route, int iterations, double p95LimitMs)
{
    var timings = new List<double>();
    var stopwatch = Stopwatch.StartNew();
    for (var i = 0; i < iterations; i++)
    {
        var requestTimer = Stopwatch.StartNew();
        using var response = await client.GetAsync(route);
        requestTimer.Stop();
        if (!response.IsSuccessStatusCode)
        {
            return PerfResult.Fail(id, name, timings, stopwatch.Elapsed, $"HTTP {(int)response.StatusCode}");
        }

        timings.Add(requestTimer.Elapsed.TotalMilliseconds);
    }

    stopwatch.Stop();
    return PerfResult.FromTimings(id, name, timings, stopwatch.Elapsed, p95LimitMs);
}

static async Task<PerfResult> MeasureStaticAssetAsync(WebAppRuntime runtime)
{
    using var client = runtime.CreateClient();
    var timings = new List<double>();
    var stopwatch = Stopwatch.StartNew();
    var hasCacheMetadata = false;

    for (var i = 0; i < 30; i++)
    {
        var requestTimer = Stopwatch.StartNew();
        using var response = await client.GetAsync(i % 2 == 0 ? "/css/site.css" : "/js/site.js");
        requestTimer.Stop();
        if (!response.IsSuccessStatusCode)
        {
            return PerfResult.Fail("PERF04", "Static asset cache latency", timings, stopwatch.Elapsed, $"HTTP {(int)response.StatusCode}");
        }

        hasCacheMetadata = hasCacheMetadata || response.Headers.ETag is not null || response.Content.Headers.LastModified is not null;
        timings.Add(requestTimer.Elapsed.TotalMilliseconds);
    }

    stopwatch.Stop();
    var result = PerfResult.FromTimings("PERF04", "Static asset cache latency", timings, stopwatch.Elapsed, p95LimitMs: 250);
    return hasCacheMetadata ? result : result with { Status = "Fail", Message = "static assets missing ETag/LastModified" };
}

static async Task<PerfResult> MeasureAuthenticatedInventoryAsync(WebAppRuntime runtime)
{
    var timings = new List<double>();
    var total = Stopwatch.StartNew();

    for (var i = 0; i < 12; i++)
    {
        using var client = runtime.CreateClient();
        var requestTimer = Stopwatch.StartNew();
        await LoginAsync(client, "duocsi@nhom4.local", "Duocsi@123");
        using var response = await client.GetAsync("/Inventory");
        requestTimer.Stop();
        if (!response.IsSuccessStatusCode)
        {
            return PerfResult.Fail("PERF05", "Authenticated inventory flow", timings, total.Elapsed, $"HTTP {(int)response.StatusCode}");
        }

        timings.Add(requestTimer.Elapsed.TotalMilliseconds);
    }

    total.Stop();
    return PerfResult.FromTimings("PERF05", "Authenticated inventory flow", timings, total.Elapsed, p95LimitMs: 900);
}

static async Task<PerfResult> MeasureConcurrencyBurstAsync(WebAppRuntime runtime)
{
    using var client = runtime.CreateClient();
    var total = Stopwatch.StartNew();
    var requests = Enumerable.Range(0, 60)
        .Select(index => MeasureOneAsync(client, index % 2 == 0 ? "/Drugs?keyword=para" : "/Drugs/Details/2"))
        .ToArray();
    var timings = await Task.WhenAll(requests);
    total.Stop();

    if (timings.Any(item => item < 0))
    {
        return PerfResult.Fail("PERF06", "Concurrency burst", timings.Where(item => item >= 0), total.Elapsed, "one or more requests failed");
    }

    return PerfResult.FromTimings("PERF06", "Concurrency burst", timings, total.Elapsed, p95LimitMs: 1500, minRps: 20);
}

static async Task<PerfResult> MeasureSustainedWindowAsync(WebAppRuntime runtime)
{
    using var client = runtime.CreateClient();
    var timings = new List<double>();
    var total = Stopwatch.StartNew();
    var deadline = DateTimeOffset.UtcNow.AddSeconds(4);

    while (DateTimeOffset.UtcNow < deadline)
    {
        foreach (var route in new[] { "/", "/Drugs?keyword=para", "/Drugs/Details/1" })
        {
            timings.Add(await MeasureOneAsync(client, route));
        }
    }

    total.Stop();
    if (timings.Any(item => item < 0))
    {
        return PerfResult.Fail("PERF07", "Realtime sustained sampling", timings.Where(item => item >= 0), total.Elapsed, "one or more requests failed");
    }

    return PerfResult.FromTimings("PERF07", "Realtime sustained sampling", timings, total.Elapsed, p95LimitMs: 800, minRps: 12);
}

static async Task<PerfResult> MeasureMemorySustainAsync(WebAppRuntime runtime)
{
    using var client = runtime.CreateClient();
    var timings = new List<double>();
    var total = Stopwatch.StartNew();

    for (var i = 0; i < 160; i++)
    {
        var route = (i % 4) switch
        {
            0 => "/",
            1 => "/Drugs?keyword=para",
            2 => "/Drugs/Details/1",
            _ => "/css/site.css"
        };
        timings.Add(await MeasureOneAsync(client, route));
    }

    total.Stop();
    if (timings.Any(item => item < 0))
    {
        return PerfResult.Fail("PERF08", "Memory sustain", timings.Where(item => item >= 0), total.Elapsed, "one or more requests failed");
    }

    var workingSetMb = runtime.WorkingSetBytes / 1024d / 1024d;
    var result = PerfResult.FromTimings("PERF08", "Memory sustain", timings, total.Elapsed, p95LimitMs: 600, minRps: 15);
    return workingSetMb < 420
        ? result with { Message = $"{result.Message}; workingSet={workingSetMb:N1}MB" }
        : result with { Status = "Fail", Message = $"working set too high: {workingSetMb:N1}MB" };
}

static async Task<PerfResult> MeasureHundredVirtualUsersAsync(WebAppRuntime runtime)
{
    const int virtualUsers = 100;
    const int requestsPerUser = 5;
    var total = Stopwatch.StartNew();
    var userTasks = Enumerable.Range(0, virtualUsers).Select(async userIndex =>
    {
        using var client = runtime.CreateClient();
        var timings = new List<double>(requestsPerUser);
        for (var requestIndex = 0; requestIndex < requestsPerUser; requestIndex++)
        {
            var route = ((userIndex + requestIndex) % 3) switch
            {
                0 => "/",
                1 => "/Drugs?keyword=para",
                _ => "/Drugs/Details/1"
            };
            timings.Add(await MeasureOneAsync(client, route));
        }

        return timings;
    });

    var timings = (await Task.WhenAll(userTasks)).SelectMany(item => item).ToArray();
    total.Stop();
    var failed = timings.Count(item => item < 0);
    var errorRate = timings.Length == 0 ? 1 : (double)failed / timings.Length;
    var successful = timings.Where(item => item >= 0).ToArray();
    var result = PerfResult.FromTimings(
        "PERF09",
        "100 virtual users",
        successful,
        total.Elapsed,
        p95LimitMs: 2_000,
        minRps: 25);

    return errorRate <= 0.01
        ? result with { Message = $"{result.Message}; users={virtualUsers}; requests={timings.Length}; errorRate={errorRate:P2}" }
        : result with { Status = "Fail", Message = $"error rate too high: {errorRate:P2}" };
}

static async Task<PerfResult> MeasureSomeeFreeProfileAsync(WebAppRuntime runtime)
{
    const int virtualUsers = 10;
    const int requestsPerUser = 12;
    var total = Stopwatch.StartNew();
    var userTasks = Enumerable.Range(0, virtualUsers).Select(async userIndex =>
    {
        using var client = runtime.CreateClient();
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip", 0.8));
        var samples = new List<(double Milliseconds, int Bytes, bool Success)>(requestsPerUser);
        for (var requestIndex = 0; requestIndex < requestsPerUser; requestIndex++)
        {
            var route = ((userIndex + requestIndex) % 3) switch
            {
                0 => "/",
                1 => "/Drugs?keyword=para",
                _ => "/Drugs/Details/1"
            };
            samples.Add(await MeasureOneWithBytesAsync(client, route));
            await Task.Delay(250);
        }

        return samples;
    });

    var samples = (await Task.WhenAll(userTasks)).SelectMany(item => item).ToArray();
    total.Stop();
    var successful = samples.Where(item => item.Success).ToArray();
    var errorRate = samples.Length == 0 ? 1 : 1 - ((double)successful.Length / samples.Length);
    var timings = successful.Select(item => item.Milliseconds).ToArray();
    var result = PerfResult.FromTimings(
        "PERF10",
        "Somee Free paced 10 virtual users",
        timings,
        total.Elapsed,
        p95LimitMs: 1_500,
        minRps: 5);
    var averageBytes = successful.Length == 0 ? 0 : successful.Average(item => item.Bytes);
    var estimatedMonthlyRequests = averageBytes <= 0
        ? 0
        : Math.Floor(5d * 1024 * 1024 * 1024 / averageBytes);
    var message =
        $"{result.Message}; users={virtualUsers}; pacing=250ms; requests={samples.Length}; " +
        $"errorRate={errorRate:P2}; avgTransfer={averageBytes:N0}B; 5GB~{estimatedMonthlyRequests:N0} responses";

    return errorRate <= 0.01
        ? result with { Message = message }
        : result with { Status = "Fail", Message = message };
}

static async Task<(double Milliseconds, int Bytes, bool Success)> MeasureOneWithBytesAsync(
    HttpClient client,
    string route)
{
    try
    {
        var stopwatch = Stopwatch.StartNew();
        using var response = await client.GetAsync(route);
        var bytes = await response.Content.ReadAsByteArrayAsync();
        stopwatch.Stop();
        return (stopwatch.Elapsed.TotalMilliseconds, bytes.Length, response.IsSuccessStatusCode);
    }
    catch (HttpRequestException)
    {
        return (-1, 0, false);
    }
    catch (TaskCanceledException)
    {
        return (-1, 0, false);
    }
}

static async Task<double> MeasureOneAsync(HttpClient client, string route)
{
    try
    {
        var stopwatch = Stopwatch.StartNew();
        using var response = await client.GetAsync(route);
        stopwatch.Stop();
        return response.IsSuccessStatusCode ? stopwatch.Elapsed.TotalMilliseconds : -1;
    }
    catch (HttpRequestException)
    {
        return -1;
    }
    catch (TaskCanceledException)
    {
        return -1;
    }
}

static async Task LoginAsync(HttpClient client, string email, string password)
{
    var loginHtml = await GetStringAsync(client, "/Auth/Login");
    var token = ExtractAntiforgeryToken(loginHtml);
    using var response = await client.PostAsync("/Auth/Login", new FormUrlEncodedContent(new Dictionary<string, string>
    {
        ["Email"] = email,
        ["Password"] = password,
        ["ReturnUrl"] = "",
        ["__RequestVerificationToken"] = token
    }));
    response.EnsureSuccessStatusCode();
}

static async Task<string> GetStringAsync(HttpClient client, string path)
{
    using var response = await client.GetAsync(path);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync();
}

static string ExtractAntiforgeryToken(string html)
{
    var input = Regex.Match(html, "<input[^>]*name=\"__RequestVerificationToken\"[^>]*>", RegexOptions.IgnoreCase);
    if (!input.Success)
    {
        throw new InvalidOperationException("anti-forgery token input missing");
    }

    var value = Regex.Match(input.Value, "value=\"([^\"]+)\"", RegexOptions.IgnoreCase);
    if (!value.Success)
    {
        throw new InvalidOperationException("anti-forgery token value missing");
    }

    return WebUtility.HtmlDecode(value.Groups[1].Value);
}

internal sealed record PerfResult(
    string Id,
    string Name,
    string Status,
    int Samples,
    double P50Ms,
    double P95Ms,
    double MaxMs,
    double RequestsPerSecond,
    string Message)
{
    public static PerfResult FromTimings(
        string id,
        string name,
        IEnumerable<double> timings,
        TimeSpan elapsed,
        double p95LimitMs,
        double minRps = 0)
    {
        var sorted = timings.OrderBy(item => item).ToArray();
        var p50 = Percentile(sorted, 0.50);
        var p95 = Percentile(sorted, 0.95);
        var max = sorted.Length == 0 ? 0 : sorted[^1];
        var rps = elapsed.TotalSeconds <= 0 ? 0 : sorted.Length / elapsed.TotalSeconds;
        var status = p95 <= p95LimitMs && rps >= minRps ? "Pass" : "Fail";
        var message = $"threshold p95<={p95LimitMs:N0}ms";
        if (minRps > 0)
        {
            message += $", rps>={minRps:N0}";
        }

        return new PerfResult(id, name, status, sorted.Length, p50, p95, max, rps, message);
    }

    public static PerfResult Fail(string id, string name, IEnumerable<double> timings, TimeSpan elapsed, string message)
    {
        var sorted = timings.OrderBy(item => item).ToArray();
        return new PerfResult(id, name, "Fail", sorted.Length, Percentile(sorted, 0.50), Percentile(sorted, 0.95), sorted.Length == 0 ? 0 : sorted[^1], elapsed.TotalSeconds <= 0 ? 0 : sorted.Length / elapsed.TotalSeconds, message);
    }

    private static double Percentile(double[] sorted, double percentile)
    {
        if (sorted.Length == 0)
        {
            return 0;
        }

        var index = Math.Clamp((int)Math.Ceiling(percentile * sorted.Length) - 1, 0, sorted.Length - 1);
        return sorted[index];
    }
}

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

    public HttpClient CreateClient()
    {
        return new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = true,
            CookieContainer = new CookieContainer()
        })
        {
            BaseAddress = BaseUri,
            Timeout = TimeSpan.FromSeconds(15)
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
