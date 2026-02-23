using System.Runtime.Serialization;

namespace Mamey.Identity.Decentralized.Exceptions;

/// <summary>
/// Represents errors related to (de)serializing DID Documents (JSON-LD, CBOR, etc).
/// </summary>
[Serializable]
public class DidSerializationException : Exception
{
    public DidSerializationException() { }

    public DidSerializationException(string message) : base(message) { }

    public DidSerializationException(string message, Exception inner) : base(message, inner) { }

    protected DidSerializationException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}