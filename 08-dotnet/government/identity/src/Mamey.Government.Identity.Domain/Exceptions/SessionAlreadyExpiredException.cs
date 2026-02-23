using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class SessionAlreadyExpiredException : DomainException
{
    public override string Code { get; } = "session_already_expired";

    public SessionAlreadyExpiredException() : base("Session is already expired.")
    {
    }
}
