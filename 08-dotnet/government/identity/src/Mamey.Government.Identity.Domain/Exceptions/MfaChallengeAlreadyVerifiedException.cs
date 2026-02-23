using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

public class MfaChallengeAlreadyVerifiedException : DomainException
{
    public MfaChallengeAlreadyVerifiedException() : base("MFA challenge has already been verified.")
    {
    }

    public MfaChallengeAlreadyVerifiedException(Guid challengeId) : base($"MFA challenge with ID '{challengeId}' has already been verified.")
    {
    }
}
