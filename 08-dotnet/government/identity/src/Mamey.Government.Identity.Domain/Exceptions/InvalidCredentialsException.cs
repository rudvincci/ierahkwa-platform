using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

public class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException() : base("Invalid credentials provided.")
    {
    }
}
