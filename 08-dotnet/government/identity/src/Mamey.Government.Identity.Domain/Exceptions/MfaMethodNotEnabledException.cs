using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MfaMethodNotEnabledException : DomainException
{
    public override string Code { get; } = "mfa_method_not_enabled";

    public MfaMethodNotEnabledException() : base("MFA method is not enabled.")
    {
    }
}
