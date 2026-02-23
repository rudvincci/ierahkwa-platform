using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Mamey.Biometrics.Services.Models;

namespace Mamey.Biometrics.Services;

/// <summary>
/// Main biometric service interface.
/// </summary>
public interface IBiometricService
{
    /// <summary>
    /// Enroll a new biometric template.
    /// </summary>
    /// <param name="request">Enrollment request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Enrollment result</returns>
    Task<EnrollmentResult> EnrollAsync(EnrollmentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verify a face against a stored template (1:1 matching).
    /// </summary>
    /// <param name="request">Verification request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Verification result</returns>
    Task<VerificationResult> VerifyAsync(VerificationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Identify a face against all stored templates (1:N matching).
    /// </summary>
    /// <param name="request">Identification request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Identification result</returns>
    Task<IdentificationResult> IdentifyAsync(IdentificationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a template by ID.
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Template or null if not found</returns>
    Task<BiometricTemplate?> GetTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get templates with pagination.
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paged result</returns>
    Task<Services.Models.PagedResult<BiometricTemplate>> GetTemplatesAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a template.
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all templates for a subject.
    /// </summary>
    /// <param name="subjectId">Subject ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of templates</returns>
    Task<List<BiometricTemplate>> GetTemplatesBySubjectAsync(Guid subjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the original image for a template.
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Image stream or null if not found</returns>
    Task<Stream?> GetOriginalImageAsync(Guid templateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete the original image for a template.
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteImageAsync(Guid templateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if the biometric engine is healthy.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if healthy, false otherwise</returns>
    Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get service statistics.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Service statistics</returns>
    Task<ServiceStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Service statistics.
/// </summary>
public class ServiceStatistics
{
    /// <summary>
    /// Total number of templates
    /// </summary>
    public long TotalTemplates { get; set; }

    /// <summary>
    /// Total number of subjects
    /// </summary>
    public long TotalSubjects { get; set; }

    /// <summary>
    /// Average quality score
    /// </summary>
    public double AverageQualityScore { get; set; }

    /// <summary>
    /// Biometric engine health status
    /// </summary>
    public bool EngineHealthy { get; set; }

    /// <summary>
    /// MongoDB health status
    /// </summary>
    public bool DatabaseHealthy { get; set; }

    /// <summary>
    /// MinIO health status
    /// </summary>
    public bool StorageHealthy { get; set; }

    /// <summary>
    /// Statistics timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
