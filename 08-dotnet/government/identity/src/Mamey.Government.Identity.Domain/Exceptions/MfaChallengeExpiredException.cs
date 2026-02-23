using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MfaChallengeExpiredException : DomainException
{
    public override string Code { get; } = "mfa_challenge_expired";

    public MfaChallengeExpiredException() : base("MFA challenge has expired.")
    {
    }

    public MfaChallengeExpiredException(Guid challengeId) : base($"MFA challenge with ID '{challengeId}' has expired.")
    {
    }
}
