using Mamey.Portal.Citizenship.Application.Models;

namespace Mamey.Portal.Citizenship.Application.Services;

/// <summary>
/// Service for managing citizenship status
/// </summary>
public interface ICitizenshipStatusService
{
    /// <summary>
    /// Gets the current citizenship status for an application by email
    /// </summary>
    Task<CitizenshipStatus?> GetStatusByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Gets the current citizenship status for an application by application ID
    /// </summary>
    Task<CitizenshipStatus?> GetStatusByApplicationIdAsync(Guid applicationId, CancellationToken ct = default);

    /// <summary>
    /// Creates or updates citizenship status when an application is approved
    /// </summary>
    Task CreateOrUpdateStatusAsync(Guid applicationId, string email, CancellationToken ct = default);

    /// <summary>
    /// Checks if a citizen is eligible to apply for the next status level
    /// </summary>
    Task<bool> IsEligibleForStatusProgressionAsync(string email, CitizenshipStatus targetStatus, CancellationToken ct = default);

    /// <summary>
    /// Gets current citizenship status details for display
    /// </summary>
    Task<CitizenshipStatusDetailsDto?> GetStatusDetailsAsync(string email, CancellationToken ct = default);
}

