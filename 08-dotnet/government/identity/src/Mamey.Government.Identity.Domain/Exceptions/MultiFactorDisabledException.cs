using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MultiFactorDisabledException : DomainException
{
    public override string Code { get; } = "multi_factor_disabled";

    public MultiFactorDisabledException() : base("Multi-factor authentication is disabled.")
    {
    }
}
