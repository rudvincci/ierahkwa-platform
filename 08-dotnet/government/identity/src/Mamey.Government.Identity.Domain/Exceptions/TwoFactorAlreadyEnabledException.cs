using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class TwoFactorAlreadyEnabledException : DomainException
{
    public override string Code { get; } = "two_factor_already_enabled";

    public TwoFactorAlreadyEnabledException() : base("Two-factor authentication is already enabled.")
    {
    }
}
