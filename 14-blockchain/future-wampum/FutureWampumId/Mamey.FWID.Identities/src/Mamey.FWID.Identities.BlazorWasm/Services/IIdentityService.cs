using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Contracts.DTOs;

namespace Mamey.FWID.Identities.BlazorWasm.Services;

/// <summary>
/// Service interface for Identity operations from BlazorWasm.
/// </summary>
public interface IIdentityService
{
    // Basic CRUD operations
    Task<List<IdentityDto>> GetIdentitiesAsync(CancellationToken cancellationToken = default);
    Task<IdentityDto?> GetIdentityAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AddIdentityAsync(AddIdentity command, CancellationToken cancellationToken = default);
    Task<bool> UpdateIdentityAsync(UpdateIdentityRequest command, CancellationToken cancellationToken = default);
    Task<bool> DeleteIdentityAsync(Guid id, CancellationToken cancellationToken = default);

    // Biometric operations
    Task<BiometricEnrollmentResultDto> EnrollBiometricAsync(Guid identityId, BiometricEnrollmentDto request, CancellationToken cancellationToken = default);
    Task<BiometricVerificationResultDto> VerifyBiometricAsync(Guid identityId, BiometricVerificationDto request, CancellationToken cancellationToken = default);
    Task<List<BiometricInfoDto>> GetBiometricsAsync(Guid identityId, CancellationToken cancellationToken = default);

    // Verification operations
    Task<VerificationResultDto> RequestVerificationAsync(Guid identityId, CancellationToken cancellationToken = default);
    Task<VerificationStatusDto> GetVerificationStatusAsync(Guid identityId, CancellationToken cancellationToken = default);

    // MFA operations
    Task<MfaSetupResultDto> EnableMfaAsync(Guid identityId, string mfaType, CancellationToken cancellationToken = default);
    Task<bool> DisableMfaAsync(Guid identityId, CancellationToken cancellationToken = default);
    Task<MfaVerificationResultDto> VerifyMfaAsync(Guid identityId, string code, CancellationToken cancellationToken = default);
}

// Additional DTOs for UI
public record BiometricEnrollmentDto(string Modality, string TemplateData);
public record BiometricEnrollmentResultDto(bool Success, string? EnrollmentId, string? ErrorMessage);
public record BiometricVerificationDto(string Modality, string TemplateData);
public record BiometricVerificationResultDto(bool IsMatch, double Confidence, string? ErrorMessage);
public record BiometricInfoDto(Guid Id, string Modality, bool IsActive, DateTime EnrolledAt);
public record VerificationResultDto(bool Success, string? RequestId, string? ErrorMessage);
public record VerificationStatusDto(string Status, DateTime? VerifiedAt, string? VerificationLevel);
public record MfaSetupResultDto(bool Success, string? QrCodeUri, string? SecretKey, string? ErrorMessage);
public record MfaVerificationResultDto(bool Success, string? ErrorMessage);

/// <summary>
/// Request to update an identity (UI-specific aggregate update command).
/// </summary>
public record UpdateIdentityRequest
{
    public Guid IdentityId { get; init; }
    public PersonalDetailsDto? PersonalDetails { get; init; }
    public ContactInformationDto? ContactInformation { get; init; }
}








