using Nhom4WebThuocThayThe.ViewModels.RoleSupport;

namespace Nhom4WebThuocThayThe.Services;

public interface IRoleDecisionSupportService
{
    IReadOnlyCollection<RoleInsightViewModel> GetInsights(string role, string? userEmail);
}
