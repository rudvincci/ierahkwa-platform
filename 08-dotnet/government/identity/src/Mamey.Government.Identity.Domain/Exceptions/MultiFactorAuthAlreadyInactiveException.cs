using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MultiFactorAuthAlreadyInactiveException : DomainException
{
    public MultiFactorAuthAlreadyInactiveException() : base("Multi-factor authentication is already inactive.")
    {
    }
}
