using Mamey.Exceptions;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Exceptions;

/// <summary>
/// Exception thrown when biometric verification fails.
/// </summary>
internal class BiometricVerificationFailedException : DomainException
{
    public override string Code { get; } = "biometric_verification_failed";
    
    public IdentityId IdentityId { get; }
    public double MatchScore { get; }
    public double Threshold { get; }

    public BiometricVerificationFailedException(IdentityId identityId, double matchScore, double threshold)
        : base($"Biometric verification failed for identity {identityId}. Match score: {matchScore:F2}, Threshold: {threshold:F2}")
    {
        IdentityId = identityId;
        MatchScore = matchScore;
        Threshold = threshold;
    }
}

