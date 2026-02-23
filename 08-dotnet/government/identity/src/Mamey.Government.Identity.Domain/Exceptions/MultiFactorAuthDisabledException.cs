using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MultiFactorAuthDisabledException : DomainException
{
    public MultiFactorAuthDisabledException() : base("Multi-factor authentication is disabled.")
    {
    }
}
