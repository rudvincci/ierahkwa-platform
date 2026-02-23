using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class TwoFactorAuthNotActiveException : DomainException
{
    public TwoFactorAuthNotActiveException() : base("Two-factor authentication is not active.")
    {
    }
}
