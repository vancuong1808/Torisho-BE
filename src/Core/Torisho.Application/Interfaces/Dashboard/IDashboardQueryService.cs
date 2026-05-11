using Torisho.Application.DTOs.Dashboard;

namespace Torisho.Application.Interfaces.Dashboard;

public interface IDashboardQueryService
{
    Task<DashboardResponseDto> GetDashboardAsync(Guid userId, int? year = null, int? month = null, CancellationToken ct = default);
}
