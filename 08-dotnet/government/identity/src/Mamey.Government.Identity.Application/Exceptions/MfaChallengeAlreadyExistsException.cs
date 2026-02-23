using Mamey.Exceptions;
using Mamey.Government.Identity.Domain.Exceptions;

namespace Mamey.Government.Identity.Application.Exceptions;

public class MfaChallengeAlreadyExistsException : DomainException
{
    public MfaChallengeAlreadyExistsException(Guid challengeId) : base($"MFA challenge with ID '{challengeId}' already exists.")
    {
    }
}
