using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class InsufficientMfaMethodsException : DomainException
{
    public override string Code { get; } = "insufficient_mfa_methods";

    public InsufficientMfaMethodsException() : base("Insufficient MFA methods enabled.")
    {
    }
}
