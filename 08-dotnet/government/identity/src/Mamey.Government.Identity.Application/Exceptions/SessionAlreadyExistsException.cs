using Mamey.Exceptions;
using Mamey.Government.Identity.Domain.Exceptions;

namespace Mamey.Government.Identity.Application.Exceptions;

public class SessionAlreadyExistsException : DomainException
{
    public SessionAlreadyExistsException(Guid sessionId) : base($"Session with ID '{sessionId}' already exists.")
    {
    }
}
