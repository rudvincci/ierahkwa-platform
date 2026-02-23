using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MfaChallengeVerificationFailedException : DomainException
{
    public override string Code { get; } = "mfa_challenge_verification_failed";

    public MfaChallengeVerificationFailedException() : base("MFA challenge verification failed.")
    {
    }
}
