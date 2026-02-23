namespace Mamey.FWID.Identities.Application.Exceptions;

/// <summary>
/// Exception thrown when credentials are invalid.
/// </summary>
public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException(string message) : base(message)
    {
    }

    public InvalidCredentialsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
