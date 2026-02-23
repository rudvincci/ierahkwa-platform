using Mamey.Portal.Citizenship.Application.Models;

namespace Mamey.Portal.Citizenship.Application.Services;

public interface IApplicationStatusService
{
    /// <summary>
    /// Get application status by application number for the current tenant.
    /// </summary>
    Task<ApplicationStatusDto?> GetApplicationStatusByNumberAsync(string applicationNumber, CancellationToken ct = default);

    /// <summary>
    /// Get application status by ID for the current tenant.
    /// </summary>
    Task<ApplicationStatusDto?> GetApplicationStatusByIdAsync(Guid applicationId, CancellationToken ct = default);

    /// <summary>
    /// Get application status by email for the current tenant (for authenticated applicants).
    /// Returns the most recent application if multiple exist.
    /// </summary>
    Task<ApplicationStatusDto?> GetApplicationStatusByEmailAsync(string email, CancellationToken ct = default);
}


