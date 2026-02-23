using Mamey.Exceptions;
using Mamey.Government.Identity.Domain.Exceptions;

namespace Mamey.Government.Identity.Application.Exceptions;

public class SessionNotFoundException : DomainException
{
    public SessionNotFoundException() : base($"Session was not found.")
    {
    }
    public SessionNotFoundException(Guid sessionId) : base($"Session with ID '{sessionId}' was not found.")
    {
    }
}
