using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Application.Mappers;
using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;

namespace Mamey.FWID.Identities.Application.Queries.Handlers;

/// <summary>
/// Handler for verifying an identity's biometric data.
/// </summary>
internal sealed class VerifyIdentityHandler : IQueryHandler<VerifyIdentity, BiometricVerificationResultDto>
{
    private readonly IIdentityRepository _repository;

    public VerifyIdentityHandler(IIdentityRepository repository)
    {
        _repository = repository;
    }

    public async Task<BiometricVerificationResultDto> HandleAsync(VerifyIdentity query, CancellationToken cancellationToken = default)
    {
        var identity = await _repository.GetAsync(query.IdentityId, cancellationToken);
        if (identity == null)
            throw new IdentityNotFoundException(query.IdentityId);

        // Business Rule: Cannot verify revoked identity
        if (identity.Status == Domain.ValueObjects.IdentityStatus.Revoked)
            throw new InvalidOperationException("Cannot verify revoked identity");

        // Map DTO to domain
        var providedBiometric = query.ProvidedBiometric.ToDomain();
        
        // Default threshold per Biometric Verification Microservice spec (§4): MATCH_MIN = 0.85
        var threshold = 0.85;
        var matchScore = identity.BiometricData.Match(providedBiometric);
        var isVerified = matchScore >= threshold;

        return new BiometricVerificationResultDto
        {
            IsVerified = isVerified,
            ConfidenceScore = matchScore * 100, // Convert to percentage
            BiometricType = Contracts.BiometricType.FaceRecognition, // Default type
            Quality = Contracts.BiometricQuality.Good,
            VerifiedAt = DateTime.UtcNow
        };
    }
}

