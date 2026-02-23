using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MultiFactorAuthNotActiveException : DomainException
{
    public MultiFactorAuthNotActiveException() : base("Multi-factor authentication is not active.")
    {
    }
}
