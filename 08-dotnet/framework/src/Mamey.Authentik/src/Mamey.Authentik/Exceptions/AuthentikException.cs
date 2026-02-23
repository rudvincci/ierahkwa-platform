namespace Mamey.Authentik.Exceptions;

/// <summary>
/// Base exception for all Authentik-related errors.
/// </summary>
public class AuthentikException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikException"/> class.
    /// </summary>
    public AuthentikException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikException"/> class with a specified error message.
    /// </summary>
    public AuthentikException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikException"/> class with a specified error message and inner exception.
    /// </summary>
    public AuthentikException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
