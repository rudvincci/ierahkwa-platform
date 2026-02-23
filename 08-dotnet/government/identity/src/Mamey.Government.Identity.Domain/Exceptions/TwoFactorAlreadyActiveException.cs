using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class TwoFactorAlreadyActiveException : DomainException
{
    public override string Code { get; } = "two_factor_already_active";

    public TwoFactorAlreadyActiveException() : base("Two-factor authentication is already active.")
    {
    }

    public TwoFactorAlreadyActiveException(Guid userId) : base($"Two-factor authentication is already active for user '{userId}'.")
    {
    }
}
