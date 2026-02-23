using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MfaChallengeNotFoundException : DomainException
{
    public override string Code { get; } = "mfa_challenge_not_found";

    public MfaChallengeNotFoundException() : base("MFA challenge not found.")
    {
    }
}
