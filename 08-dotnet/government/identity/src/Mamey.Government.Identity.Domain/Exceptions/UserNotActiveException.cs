using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

public class UserNotActiveException : DomainException
{
    public UserNotActiveException() : base("User is not active.")
    {
    }

    public UserNotActiveException(Guid userId) : base($"User with ID '{userId}' is not active.")
    {
    }
}
