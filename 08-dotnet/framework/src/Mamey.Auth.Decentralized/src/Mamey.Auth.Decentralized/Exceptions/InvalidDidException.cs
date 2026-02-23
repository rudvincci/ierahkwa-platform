using System;

namespace Mamey.Auth.Decentralized.Exceptions;

/// <summary>
/// Exception thrown when a DID format is invalid.
/// </summary>
public class InvalidDidException : DidException
{
    public InvalidDidException(string message) : base(message)
    {
    }

    public InvalidDidException(string message, Exception innerException) : base(message, innerException)
    {
    }
}