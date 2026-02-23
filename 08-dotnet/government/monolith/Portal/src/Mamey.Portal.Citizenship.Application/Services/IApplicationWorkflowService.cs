namespace Mamey.Portal.Citizenship.Application.Services;

/// <summary>
/// Service for managing application workflow transitions
/// </summary>
public interface IApplicationWorkflowService
{
    /// <summary>
    /// Processes the next workflow step for an application
    /// </summary>
    Task<bool> ProcessNextWorkflowStepAsync(Guid applicationId, CancellationToken ct = default);

    /// <summary>
    /// Validates an application (placeholder for AI-powered verification)
    /// </summary>
    Task<bool> ValidateApplicationAsync(Guid applicationId, CancellationToken ct = default);

    /// <summary>
    /// Processes KYC for an application (placeholder for AI-powered KYC)
    /// </summary>
    Task<bool> ProcessKycAsync(Guid applicationId, CancellationToken ct = default);
}

