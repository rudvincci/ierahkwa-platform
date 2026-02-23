using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MultiFactorAlreadyInactiveException : DomainException
{
    public override string Code { get; } = "multi_factor_already_inactive";

    public MultiFactorAlreadyInactiveException() : base("Multi-factor authentication is already inactive.")
    {
    }

    public MultiFactorAlreadyInactiveException(UserId userId) 
        : base($"Multi-factor authentication is already inactive for user '{userId}'.")
    {
    }
}
