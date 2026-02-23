using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class TwoFactorAlreadyDisabledException : DomainException
{
    public override string Code { get; } = "two_factor_already_disabled";

    public TwoFactorAlreadyDisabledException() : base("Two-factor authentication is already disabled.")
    {
    }

    public TwoFactorAlreadyDisabledException(Guid userId) : base($"Two-factor authentication is already disabled for user '{userId}'.")
    {
    }
}
