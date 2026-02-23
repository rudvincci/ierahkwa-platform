using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

public class SessionNotActiveException : DomainException
{
    public SessionNotActiveException() : base("Session is not active.")
    {
    }

    public SessionNotActiveException(Guid sessionId) : base($"Session with ID '{sessionId}' is not active.")
    {
    }
}