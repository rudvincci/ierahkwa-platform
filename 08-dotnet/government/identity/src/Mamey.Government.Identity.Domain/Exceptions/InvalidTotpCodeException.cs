using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class InvalidTotpCodeException : DomainException
{
    public override string Code { get; } = "invalid_totp_code";

    public InvalidTotpCodeException() : base("TOTP code is invalid.")
    {
    }
}
