using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class TwoFactorDisabledException : DomainException
{
    public override string Code { get; } = "two_factor_disabled";

    public TwoFactorDisabledException() : base("Two-factor authentication is disabled.")
    {
    }
}
