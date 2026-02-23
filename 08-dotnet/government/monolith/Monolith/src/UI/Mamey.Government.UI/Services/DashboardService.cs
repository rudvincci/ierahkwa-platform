using Mamey.Government.UI.Models;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.UI.Services;

/// <summary>
/// Dashboard service that aggregates statistics from multiple APIs.
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly IApplicationsService _applicationsService;
    private readonly ICitizensService _citizensService;
    private readonly IPassportsService _passportsService;
    private readonly ITravelIdentitiesService _travelIdentitiesService;
    private readonly ICertificatesService _certificatesService;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        IApplicationsService applicationsService,
        ICitizensService citizensService,
        IPassportsService passportsService,
        ITravelIdentitiesService travelIdentitiesService,
        ICertificatesService certificatesService,
        ILogger<DashboardService> logger)
    {
        _applicationsService = applicationsService;
        _citizensService = citizensService;
        _passportsService = passportsService;
        _travelIdentitiesService = travelIdentitiesService;
        _certificatesService = certificatesService;
        _logger = logger;
    }

    public async Task<DashboardStats> GetStatsAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var stats = new DashboardStats();
        
        try
        {
            // Fetch all stats in parallel for better performance
            var applicationsTask = GetApplicationStatsAsync(tenantId, cancellationToken);
            var citizensTask = GetCitizenStatsAsync(tenantId, cancellationToken);
            var passportsTask = GetPassportStatsAsync(tenantId, cancellationToken);
            var travelIdsTask = GetTravelIdStatsAsync(tenantId, cancellationToken);
            var certificatesTask = GetCertificateStatsAsync(tenantId, cancellationToken);

            await Task.WhenAll(applicationsTask, citizensTask, passportsTask, travelIdsTask, certificatesTask);

            // Merge results
            var appStats = await applicationsTask;
            var citizenStats = await citizensTask;
            var passportStats = await passportsTask;
            var travelIdStats = await travelIdsTask;
            var certStats = await certificatesTask;

            stats.PendingApplications = appStats.Pending;
            stats.InReviewApplications = appStats.InReview;
            stats.ApprovedApplications = appStats.Approved;
            stats.RejectedApplications = appStats.Rejected;
            stats.RecentApplications = appStats.Recent;

            stats.TotalCitizens = citizenStats.Total;
            stats.ProbationaryCitizens = citizenStats.Probationary;
            stats.ResidentCitizens = citizenStats.Resident;
            stats.FullCitizens = citizenStats.Full;

            stats.ActivePassports = passportStats.Active;
            stats.ExpiringPassports = passportStats.Expiring;
            stats.ExpiredPassports = passportStats.Expired;

            stats.ActiveTravelIds = travelIdStats.Active;
            stats.ExpiringTravelIds = travelIdStats.Expiring;
            stats.ExpiredTravelIds = travelIdStats.Expired;

            stats.ActiveCertificates = certStats.Active;
            stats.ArchivedCertificates = certStats.Archived;
            stats.RevokedCertificates = certStats.Revoked;

            stats.DocumentsIssued = passportStats.Active + travelIdStats.Active + certStats.Active;
            stats.ProgressionRequests = 0; // TODO: Add progression tracking
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching dashboard stats for tenant {TenantId}", tenantId);
        }

        return stats;
    }

    private async Task<(int Pending, int InReview, int Approved, int Rejected, List<ApplicationSummaryModel> Recent)> GetApplicationStatsAsync(
        Guid tenantId, CancellationToken cancellationToken)
    {
        try
        {
            var pendingResult = await _applicationsService.BrowseAsync(tenantId, "Submitted", null, null, null, 1, 1, cancellationToken);
            var inReviewResult = await _applicationsService.BrowseAsync(tenantId, "InReview", null, null, null, 1, 1, cancellationToken);
            var approvedResult = await _applicationsService.BrowseAsync(tenantId, "Approved", null, null, null, 1, 1, cancellationToken);
            var rejectedResult = await _applicationsService.BrowseAsync(tenantId, "Rejected", null, null, null, 1, 1, cancellationToken);
            var recentResult = await _applicationsService.BrowseAsync(tenantId, null, null, null, null, 1, 5, cancellationToken);

            return (
                (int)pendingResult.TotalResults,
                (int)inReviewResult.TotalResults,
                (int)approvedResult.TotalResults,
                (int)rejectedResult.TotalResults,
                recentResult.Items.ToList()
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch application stats");
            return (0, 0, 0, 0, new List<ApplicationSummaryModel>());
        }
    }

    private async Task<(int Total, int Probationary, int Resident, int Full)> GetCitizenStatsAsync(
        Guid tenantId, CancellationToken cancellationToken)
    {
        try
        {
            var allResult = await _citizensService.BrowseAsync(tenantId, null, null, null, 1, 1, cancellationToken);
            var probResult = await _citizensService.BrowseAsync(tenantId, "Probationary", null, null, 1, 1, cancellationToken);
            var resResult = await _citizensService.BrowseAsync(tenantId, "Resident", null, null, 1, 1, cancellationToken);
            var fullResult = await _citizensService.BrowseAsync(tenantId, "Citizen", null, null, 1, 1, cancellationToken);

            return (
                (int)allResult.TotalResults,
                (int)probResult.TotalResults,
                (int)resResult.TotalResults,
                (int)fullResult.TotalResults
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch citizen stats");
            return (0, 0, 0, 0);
        }
    }

    private async Task<(int Active, int Expiring, int Expired)> GetPassportStatsAsync(
        Guid tenantId, CancellationToken cancellationToken)
    {
        try
        {
            var activeResult = await _passportsService.BrowseAsync(tenantId, "Active", null, 1, 1, cancellationToken);
            var expiringResult = await _passportsService.BrowseAsync(tenantId, "Expiring", null, 1, 1, cancellationToken);
            var expiredResult = await _passportsService.BrowseAsync(tenantId, "Expired", null, 1, 1, cancellationToken);

            return (
                (int)activeResult.TotalResults,
                (int)expiringResult.TotalResults,
                (int)expiredResult.TotalResults
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch passport stats");
            return (0, 0, 0);
        }
    }

    private async Task<(int Active, int Expiring, int Expired)> GetTravelIdStatsAsync(
        Guid tenantId, CancellationToken cancellationToken)
    {
        try
        {
            var activeResult = await _travelIdentitiesService.BrowseAsync(tenantId, "Active", null, 1, 1, cancellationToken);
            var expiringResult = await _travelIdentitiesService.BrowseAsync(tenantId, "Expiring", null, 1, 1, cancellationToken);
            var expiredResult = await _travelIdentitiesService.BrowseAsync(tenantId, "Expired", null, 1, 1, cancellationToken);

            return (
                (int)activeResult.TotalResults,
                (int)expiringResult.TotalResults,
                (int)expiredResult.TotalResults
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch travel ID stats");
            return (0, 0, 0);
        }
    }

    private async Task<(int Active, int Archived, int Revoked)> GetCertificateStatsAsync(
        Guid tenantId, CancellationToken cancellationToken)
    {
        try
        {
            // Note: certificateType=null, status="Active"/"Archived"/"Revoked"
            var activeResult = await _certificatesService.BrowseAsync(tenantId, null, "Active", null, 1, 1, cancellationToken);
            var archivedResult = await _certificatesService.BrowseAsync(tenantId, null, "Archived", null, 1, 1, cancellationToken);
            var revokedResult = await _certificatesService.BrowseAsync(tenantId, null, "Revoked", null, 1, 1, cancellationToken);

            return (
                (int)activeResult.TotalResults,
                (int)archivedResult.TotalResults,
                (int)revokedResult.TotalResults
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch certificate stats");
            return (0, 0, 0);
        }
    }
}
