using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.Localization;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;
using Nhom4WebThuocThayThe.Services.Ai;
using System.Globalization;
using System.IO.Compression;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services
    .AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (_, factory) =>
            factory.Create(typeof(SharedResource));
    });
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var vietnamese = new CultureInfo("vi-VN");
    options.DefaultRequestCulture = new RequestCulture(vietnamese);
    options.SupportedCultures = [vietnamese];
    options.SupportedUICultures = [vietnamese];
    options.ApplyCurrentCultureToResponseHeaders = true;
    options.RequestCultureProviders.Clear();
});
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    options.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    options.Level = CompressionLevel.Fastest);
builder.Services.AddMemoryCache();
builder.Services.Configure<GeminiOptions>(
    builder.Configuration.GetSection(GeminiOptions.SectionName));
builder.Services.AddHttpClient<IAiRecommendationExplanationService, GeminiAiRecommendationExplanationService>(client =>
{
    client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/models/");
});
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });
var relaxLoginRateLimit = string.Equals(
    builder.Configuration["Testing:RelaxLoginRateLimit"],
    "true",
    StringComparison.OrdinalIgnoreCase);
var loginPermitLimit = relaxLoginRateLimit ? 200 : 20;
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("login", httpContext =>
    {
        var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = loginPermitLimit,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });
    options.AddPolicy("ai", httpContext =>
    {
        var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(
            $"ai:{partitionKey}",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(AppRoles.Admin));
    options.AddPolicy("CatalogManager", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Pharmacist));
    options.AddPolicy("InventoryManager", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Pharmacist));
    options.AddPolicy("ExpertReviewer", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Expert, AppRoles.Pharmacist));
});
var encodedAccounts = builder.Configuration["Authentication:EncodedAccounts"];
if (!string.IsNullOrWhiteSpace(encodedAccounts))
{
    var accounts = ConfiguredUserAccountLoader.Load(encodedAccounts);
    builder.Services.AddSingleton<IUserAccountService>(new InMemoryUserAccountService(accounts));
}
else if (builder.Environment.IsProduction())
{
    throw new InvalidOperationException(
        "Production authentication is not configured. Set Authentication__EncodedAccounts.");
}
else
{
    builder.Services.AddSingleton<IUserAccountService>(new InMemoryUserAccountService());
}
builder.Services.AddDbContext<PharmacyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PharmacyDatabase")));
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IDrugCatalogService, DrugCatalogService>();
builder.Services.AddScoped<IDrugSearchService, DrugSearchService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IReportingService, ReportingService>();
builder.Services.AddScoped<IExpertReviewService, ExpertReviewService>();
builder.Services.AddScoped<IRoleDecisionSupportService, RoleDecisionSupportService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    PharmacyDbInitializer.Initialize(scope.ServiceProvider.GetRequiredService<PharmacyDbContext>());
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
        context.Context.Response.Headers.CacheControl = "public,max-age=604800"
});

var localizationOptions = app.Services
    .GetRequiredService<IOptions<RequestLocalizationOptions>>()
    .Value;
app.UseRequestLocalization(localizationOptions);
app.UseRouting();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self'; base-uri 'self'; form-action 'self'; frame-ancestors 'none'; " +
        "object-src 'none'; img-src 'self' data:; style-src 'self' 'unsafe-inline'; script-src 'self' 'unsafe-inline'";
    await next();
});

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", async (PharmacyDbContext dbContext) =>
    await dbContext.Database.CanConnectAsync()
        ? Results.Ok(new { status = "healthy", database = "connected" })
        : Results.Problem("Database connection failed."));

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
