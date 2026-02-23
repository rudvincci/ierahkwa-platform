using Mamey.Exceptions;
using Mamey.Government.Identity.Domain.Exceptions;

namespace Mamey.Government.Identity.Application.Exceptions;

public class MfaChallengeNotFoundException : DomainException
{
    public MfaChallengeNotFoundException(Guid challengeId) : base($"MFA challenge with ID '{challengeId}' was not found.")
    {
    }
}
