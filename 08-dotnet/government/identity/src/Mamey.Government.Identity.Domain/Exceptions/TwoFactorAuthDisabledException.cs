using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class TwoFactorAuthDisabledException : DomainException
{
    public TwoFactorAuthDisabledException() : base("Two-factor authentication is disabled.")
    {
    }
}
