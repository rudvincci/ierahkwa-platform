using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MfaMethodAlreadyEnabledException : DomainException
{
    public override string Code { get; } = "mfa_method_already_enabled";

    public MfaMethodAlreadyEnabledException() : base("MFA method is already enabled.")
    {
    }
}
