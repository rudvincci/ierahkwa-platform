using Mamey.Types;

namespace Mamey.Auth.DecentralizedIdentifiers.Audit;

/// <summary>
/// Service for logging DID-related audit events.
/// </summary>
public interface IDidAuditService
{
    /// <summary>
    /// Logs a DID operation audit event.
    /// </summary>
    /// <param name="activityType">The type of activity being logged.</param>
    /// <param name="category">The category of the activity.</param>
    /// <param name="status">The status of the operation.</param>
    /// <param name="did">The DID involved in the operation.</param>
    /// <param name="correlationId">The correlation ID for tracking related operations.</param>
    /// <param name="userId">The user ID (optional).</param>
    /// <param name="email">The user email (optional).</param>
    /// <param name="name">The user name (optional).</param>
    /// <param name="metadata">Additional metadata for the audit event.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the async operation.</returns>
    Task LogDidOperationAsync(
        string activityType,
        string category,
        string status,
        string did,
        string correlationId,
        UserId? userId = null,
        Email? email = null,
        Name? name = null,
        Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a Verifiable Credential operation audit event.
    /// </summary>
    /// <param name="activityType">The type of activity being logged.</param>
    /// <param name="category">The category of the activity.</param>
    /// <param name="status">The status of the operation.</param>
    /// <param name="credentialId">The credential ID involved in the operation.</param>
    /// <param name="correlationId">The correlation ID for tracking related operations.</param>
    /// <param name="userId">The user ID (optional).</param>
    /// <param name="email">The user email (optional).</param>
    /// <param name="name">The user name (optional).</param>
    /// <param name="metadata">Additional metadata for the audit event.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the async operation.</returns>
    Task LogCredentialOperationAsync(
        string activityType,
        string category,
        string status,
        string credentialId,
        string correlationId,
        UserId? userId = null,
        Email? email = null,
        Name? name = null,
        Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a Verifiable Presentation operation audit event.
    /// </summary>
    /// <param name="activityType">The type of activity being logged.</param>
    /// <param name="category">The category of the activity.</param>
    /// <param name="status">The status of the operation.</param>
    /// <param name="presentationId">The presentation ID involved in the operation.</param>
    /// <param name="correlationId">The correlation ID for tracking related operations.</param>
    /// <param name="userId">The user ID (optional).</param>
    /// <param name="email">The user email (optional).</param>
    /// <param name="name">The user name (optional).</param>
    /// <param name="metadata">Additional metadata for the audit event.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the async operation.</returns>
    Task LogPresentationOperationAsync(
        string activityType,
        string category,
        string status,
        string presentationId,
        string correlationId,
        UserId? userId = null,
        Email? email = null,
        Name? name = null,
        Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs an authentication event.
    /// </summary>
    /// <param name="activityType">The type of activity being logged.</param>
    /// <param name="category">The category of the activity.</param>
    /// <param name="status">The status of the operation.</param>
    /// <param name="subject">The subject being authenticated.</param>
    /// <param name="correlationId">The correlation ID for tracking related operations.</param>
    /// <param name="userId">The user ID (optional).</param>
    /// <param name="email">The user email (optional).</param>
    /// <param name="name">The user name (optional).</param>
    /// <param name="metadata">Additional metadata for the audit event.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the async operation.</returns>
    Task LogAuthenticationEventAsync(
        string activityType,
        string category,
        string status,
        string subject,
        string correlationId,
        UserId? userId = null,
        Email? email = null,
        Name? name = null,
        Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a general DID-related event with custom parameters.
    /// </summary>
    /// <param name="activityType">The type of activity being logged.</param>
    /// <param name="category">The category of the activity.</param>
    /// <param name="status">The status of the operation.</param>
    /// <param name="subject">The subject of the operation.</param>
    /// <param name="correlationId">The correlation ID for tracking related operations.</param>
    /// <param name="userId">The user ID (optional).</param>
    /// <param name="email">The user email (optional).</param>
    /// <param name="name">The user name (optional).</param>
    /// <param name="metadata">Additional metadata for the audit event.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the async operation.</returns>
    Task LogEventAsync(
        string activityType,
        string category,
        string status,
        string subject,
        string correlationId,
        UserId? userId = null,
        Email? email = null,
        Name? name = null,
        Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default);
}







