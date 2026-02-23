using Mamey.Government.UI.Models;

namespace Mamey.Government.UI.Services;

/// <summary>
/// Service for fetching dashboard statistics.
/// </summary>
public interface IDashboardService
{
    Task<DashboardStats> GetStatsAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
