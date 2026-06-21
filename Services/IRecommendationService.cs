using Nhom4WebThuocThayThe.ViewModels.Search;

namespace Nhom4WebThuocThayThe.Services;

public interface IRecommendationService
{
    IReadOnlyCollection<DrugRecommendationViewModel> GetRecommendations(int drugId, string? userEmail);

    Task<IReadOnlyCollection<DrugRecommendationViewModel>> GetRecommendationsAsync(int drugId, string? userEmail);
}
