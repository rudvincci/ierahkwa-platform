using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

public class UserAlreadyExistsException : DomainException
{
    public UserAlreadyExistsException() : base("User already exists.")
    {
    }

    public UserAlreadyExistsException(string email) : base($"User with email '{email}' already exists.")
    {
    }
}
