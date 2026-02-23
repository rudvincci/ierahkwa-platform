using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class TwoFactorAuthAlreadyActiveException : DomainException
{
    public TwoFactorAuthAlreadyActiveException() : base("Two-factor authentication is already active.")
    {
    }
}
