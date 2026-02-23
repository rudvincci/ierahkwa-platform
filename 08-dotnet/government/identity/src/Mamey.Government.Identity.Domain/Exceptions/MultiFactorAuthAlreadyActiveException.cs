using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MultiFactorAuthAlreadyActiveException : DomainException
{
    public MultiFactorAuthAlreadyActiveException() : base("Multi-factor authentication is already active.")
    {
    }
}
