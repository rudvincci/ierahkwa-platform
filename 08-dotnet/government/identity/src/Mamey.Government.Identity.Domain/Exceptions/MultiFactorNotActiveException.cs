using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MultiFactorNotActiveException : DomainException
{
    public override string Code { get; } = "multi_factor_not_active";

    public MultiFactorNotActiveException() : base("Multi-factor authentication is not active.")
    {
    }
}
