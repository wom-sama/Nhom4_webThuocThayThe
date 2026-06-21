using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Nhom4WebThuocThayThe.Controllers;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;
using Nhom4WebThuocThayThe.Services.Ai;
using Nhom4WebThuocThayThe.ViewModels.Search;

namespace Nhom4WebThuocThayThe.UnitTests;

public sealed class DrugsControllerAiTests
{
    [Fact]
    public async Task ExplainAlternativeBuildsGenericContextWithoutPersonalProfile()
    {
        var search = new FakeSearchService { Detail = CreateDetail() };
        var ai = new CapturingAiService();
        var controller = new DrugsController(search, ai);

        var result = await controller.ExplainAlternative(1, 2, CancellationToken.None);

        Assert.IsType<JsonResult>(result);
        Assert.Null(search.LastUserEmail);
        Assert.NotNull(ai.Context);
        var serialized = System.Text.Json.JsonSerializer.Serialize(ai.Context);
        Assert.DoesNotContain("patient@example.com", serialized, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Ho so rieng", serialized, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(78, ai.Context.DeterministicScore);
    }

    [Fact]
    public async Task ExplainAlternativeReturnsNotFoundForUnknownCandidate()
    {
        var controller = new DrugsController(
            new FakeSearchService { Detail = CreateDetail() },
            new CapturingAiService());

        var result = await controller.ExplainAlternative(1, 999, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void ExplainAlternativeRequiresAntiforgeryAndAiRateLimit()
    {
        var method = typeof(DrugsController).GetMethod(nameof(DrugsController.ExplainAlternative));

        Assert.NotNull(method);
        Assert.NotNull(method.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>());
        Assert.NotNull(method.GetCustomAttribute<EnableRateLimitingAttribute>());
    }

    private static DrugDetailViewModel CreateDetail() => new()
    {
        Id = 1,
        Name = "Panadol",
        Strength = "500 mg",
        Category = "Giam dau",
        DosageForm = "Vien nen",
        Unit = "Vien",
        Manufacturer = "Demo",
        ActiveIngredient = "Paracetamol",
        SafetyProfileName = "Ho so rieng",
        SafetyProfileNote = "patient@example.com",
        Alternatives =
        [
            new DrugRecommendationViewModel
            {
                Id = 2,
                Name = "Paracetamol STADA",
                Strength = "500 mg",
                DosageForm = "Vien nen",
                Manufacturer = "Demo",
                ActiveIngredient = "Paracetamol",
                StockQuantity = 20,
                Score = 78,
                ScoreLabel = "Phu hop",
                Reasons = ["Cung hoat chat."],
                Alerts = [new SafetyAlert { Severity = "Low", Title = "Luu y", Message = "Doc huong dan." }]
            }
        ]
    };

    private sealed class FakeSearchService : IDrugSearchService
    {
        public DrugDetailViewModel? Detail { get; init; }

        public string? LastUserEmail { get; private set; }

        public Task<DrugSearchPageViewModel> SearchAsync(string? keyword, int? categoryId) =>
            Task.FromResult(new DrugSearchPageViewModel());

        public Task<DrugDetailViewModel?> GetDetailAsync(int id, string? userEmail)
        {
            LastUserEmail = userEmail;
            return Task.FromResult(Detail);
        }
    }

    private sealed class CapturingAiService : IAiRecommendationExplanationService
    {
        public bool IsEnabled => true;

        public AiRecommendationContext? Context { get; private set; }

        public Task<AiExplanationResult> ExplainAsync(
            AiRecommendationContext context,
            CancellationToken cancellationToken = default)
        {
            Context = context;
            return Task.FromResult(new AiExplanationResult(
                "Tom tat",
                ["Kiem tra"],
                "Gioi han",
                true,
                "Fake",
                "fake-model"));
        }
    }
}
