using Mamey.Portal.Citizenship.Application.Models;

namespace Mamey.Portal.Citizenship.Application.Services;

/// <summary>
/// Service for managing citizenship status progression applications
/// </summary>
public interface IStatusProgressionService
{
    /// <summary>
    /// Checks if a citizen is eligible to apply for the next status level
    /// </summary>
    Task<StatusProgressionEligibilityDto?> CheckEligibilityAsync(string email, CitizenshipStatus targetStatus, CancellationToken ct = default);

    /// <summary>
    /// Submits a status progression application
    /// </summary>
    Task<string> SubmitProgressionApplicationAsync(string email, CitizenshipStatus targetStatus, CancellationToken ct = default);

    /// <summary>
    /// Gets all progression applications for a citizen
    /// </summary>
    Task<IReadOnlyList<StatusProgressionApplicationDto>> GetProgressionApplicationsAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Approves a status progression application and updates citizenship status
    /// </summary>
    Task ApproveProgressionApplicationAsync(Guid progressionApplicationId, CancellationToken ct = default);

    /// <summary>
    /// Gets the status progression timeline for a citizen
    /// </summary>
    Task<StatusProgressionTimelineDto> GetStatusProgressionTimelineAsync(string email, CancellationToken ct = default);
}


