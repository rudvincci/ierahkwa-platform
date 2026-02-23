using System.Runtime.Serialization;

namespace Mamey.Identity.Decentralized.Exceptions;

/// <summary>
/// Thrown when a DID or DID URL is malformed or violates the W3C syntax.
/// </summary>
[Serializable]
public class InvalidDidFormatException : Exception
{
    public InvalidDidFormatException() { }

    public InvalidDidFormatException(string message) : base(message) { }

    public InvalidDidFormatException(string message, Exception inner) : base(message, inner) { }

    protected InvalidDidFormatException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}