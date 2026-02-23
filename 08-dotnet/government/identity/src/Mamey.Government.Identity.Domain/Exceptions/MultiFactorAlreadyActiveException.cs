using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MultiFactorAlreadyActiveException : DomainException
{
    public override string Code { get; } = "multi_factor_already_active";

    public MultiFactorAlreadyActiveException() : base("Multi-factor authentication is already active.")
    {
    }

    public MultiFactorAlreadyActiveException(UserId userId) 
        : base($"Multi-factor authentication is already active for user '{userId}'.")
    {
    }
}
