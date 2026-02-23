using System.Runtime.Serialization;

namespace Mamey.Auth.DecentralizedIdentifiers.Exceptions;

/// <summary>
/// Thrown when a requested DID method is not supported or not registered.
/// </summary>
[Serializable]
public class DidMethodNotSupportedException : Exception
{
    public DidMethodNotSupportedException() { }

    public DidMethodNotSupportedException(string message) : base(message) { }

    public DidMethodNotSupportedException(string message, Exception inner) : base(message, inner) { }

    protected DidMethodNotSupportedException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}