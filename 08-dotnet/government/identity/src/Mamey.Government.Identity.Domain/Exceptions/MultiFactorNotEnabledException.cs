using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MultiFactorNotEnabledException : DomainException
{
    public override string Code { get; } = "multi_factor_not_enabled";

    public MultiFactorNotEnabledException() : base("Multi-factor authentication is not enabled.")
    {
    }
}
