using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

public class UserNotFoundException : DomainException
{
    public UserNotFoundException() : base("User not found.")
    {
    }

    public UserNotFoundException(Guid userId) : base($"User with ID '{userId}' not found.")
    {
    }
    public UserNotFoundException(string email) : base($"User with email '{email}' not found.")
    {
    }
}
