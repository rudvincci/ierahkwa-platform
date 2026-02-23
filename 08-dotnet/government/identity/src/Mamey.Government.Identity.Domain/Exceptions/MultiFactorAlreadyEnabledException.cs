using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MultiFactorAlreadyEnabledException : DomainException
{
    public override string Code { get; } = "multi_factor_already_enabled";

    public MultiFactorAlreadyEnabledException() : base("Multi-factor authentication is already enabled.")
    {
    }

    public MultiFactorAlreadyEnabledException(Guid userId) : base($"Multi-factor authentication is already enabled for user '{userId}'.")
    {
    }
}
