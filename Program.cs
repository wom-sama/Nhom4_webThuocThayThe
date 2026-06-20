using Microsoft.AspNetCore.Authentication.Cookies;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(AppRoles.Admin));
    options.AddPolicy("CatalogManager", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Pharmacist));
    options.AddPolicy("InventoryManager", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Pharmacist));
    options.AddPolicy("ExpertReviewer", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Expert, AppRoles.Pharmacist));
});
builder.Services.AddSingleton<IUserAccountService, InMemoryUserAccountService>();
builder.Services.AddSingleton<InMemoryPharmacyStore>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IDrugCatalogService, DrugCatalogService>();
builder.Services.AddScoped<IDrugSearchService, DrugSearchService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IReportingService, ReportingService>();
builder.Services.AddScoped<IExpertReviewService, ExpertReviewService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
