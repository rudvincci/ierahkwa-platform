using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class SessionAlreadyRevokedException : DomainException
{
    public override string Code { get; } = "session_already_revoked";

    public SessionAlreadyRevokedException() : base("Session is already revoked.")
    {
    }
}
