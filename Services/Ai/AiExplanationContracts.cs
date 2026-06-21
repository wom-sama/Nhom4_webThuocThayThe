namespace Nhom4WebThuocThayThe.Services.Ai;

public sealed record AiRecommendationContext(
    string SourceDrug,
    string SourceStrength,
    string CandidateDrug,
    string CandidateStrength,
    string CandidateActiveIngredient,
    string CandidateDosageForm,
    bool PrescriptionRequired,
    int StockQuantity,
    int DeterministicScore,
    IReadOnlyCollection<string> DeterministicReasons,
    IReadOnlyCollection<string> GeneralSafetyAlerts);

public sealed record AiExplanationResult(
    string Summary,
    IReadOnlyCollection<string> Checkpoints,
    string Limitations,
    bool IsAiGenerated,
    string Provider,
    string Model);

public interface IAiRecommendationExplanationService
{
    bool IsEnabled { get; }

    Task<AiExplanationResult> ExplainAsync(
        AiRecommendationContext context,
        CancellationToken cancellationToken = default);
}
