using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class TwoFactorAuthAlreadyDisabledException : DomainException
{
    public TwoFactorAuthAlreadyDisabledException() : base("Two-factor authentication is already disabled.")
    {
    }
}
