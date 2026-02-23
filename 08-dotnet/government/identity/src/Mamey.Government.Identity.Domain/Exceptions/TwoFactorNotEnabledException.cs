using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class TwoFactorNotEnabledException : DomainException
{
    public override string Code { get; } = "two_factor_not_enabled";

    public TwoFactorNotEnabledException() : base("Two-factor authentication is not enabled.")
    {
    }
}
