using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MultiFactorAlreadyDisabledException : DomainException
{
    public override string Code { get; } = "multi_factor_already_disabled";

    public MultiFactorAlreadyDisabledException() : base("Multi-factor authentication is already disabled.")
    {
    }
}
