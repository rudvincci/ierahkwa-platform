using Mamey.FWID.Identities.Application.AI.Models;

namespace Mamey.FWID.Identities.Application.AI.Services;

/// <summary>
/// Interface for AI-powered identity verification orchestration service.
/// </summary>
public interface IAIIdentityVerificationService
{
    /// <summary>
    /// Performs complete AI-powered identity verification.
    /// </summary>
    Task<VerificationResult> VerifyIdentityAsync(
        IdentityVerificationRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets verification result by ID.
    /// </summary>
    Task<VerificationResult?> GetVerificationResultAsync(
        Guid verificationId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets verification history for an identity.
    /// </summary>
    Task<IReadOnlyList<VerificationResult>> GetVerificationHistoryAsync(
        Guid identityId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Re-runs verification with updated data.
    /// </summary>
    Task<VerificationResult> ReVerifyAsync(
        Guid verificationId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Complete identity verification request.
/// </summary>
public class IdentityVerificationRequest
{
    public Guid IdentityId { get; set; }
    
    // Document verification
    public byte[]? DocumentFrontImage { get; set; }
    public byte[]? DocumentBackImage { get; set; }
    public DocumentClassification? ExpectedDocumentType { get; set; }
    
    // Biometric verification
    public byte[]? SelfieImage { get; set; }
    public bool PerformLivenessCheck { get; set; } = true;
    
    // Fraud detection context
    public string? IPAddress { get; set; }
    public DeviceFingerprint? DeviceFingerprint { get; set; }
    public BehaviorData? BehaviorData { get; set; }
    
    // Identity data for matching
    public string? ExpectedFirstName { get; set; }
    public string? ExpectedLastName { get; set; }
    public DateTime? ExpectedDateOfBirth { get; set; }
}
