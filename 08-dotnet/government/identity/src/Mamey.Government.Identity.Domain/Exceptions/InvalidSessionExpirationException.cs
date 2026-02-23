using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class InvalidSessionExpirationException : DomainException
{
    public override string Code { get; } = "invalid_session_expiration";

    public InvalidSessionExpirationException() : base("Session expiration time is invalid.")
    {
    }
}
