using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

public class InvalidRefreshTokenException : DomainException
{
    public InvalidRefreshTokenException() : base("Invalid refresh token provided.")
    {
    }
}
