using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class TwoFactorNotActiveException : DomainException
{
    public override string Code { get; } = "two_factor_not_active";

    public TwoFactorNotActiveException() : base("Two-factor authentication is not active.")
    {
    }

    public TwoFactorNotActiveException(Guid userId) : base($"Two-factor authentication is not active for user '{userId}'.")
    {
    }
}
