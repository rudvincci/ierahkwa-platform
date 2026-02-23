using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MultiFactorAuthAlreadyDisabledException : DomainException
{
    public MultiFactorAuthAlreadyDisabledException() : base("Multi-factor authentication is already disabled.")
    {
    }
}
