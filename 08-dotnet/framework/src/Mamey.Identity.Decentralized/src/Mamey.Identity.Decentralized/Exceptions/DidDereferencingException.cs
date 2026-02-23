using System.Runtime.Serialization;

namespace Mamey.Identity.Decentralized.Exceptions;

/// <summary>
/// Represents errors encountered during DID URL dereferencing.
/// </summary>
[Serializable]
public class DidDereferencingException : Exception
{
    public DidDereferencingException() { }

    public DidDereferencingException(string message) : base(message) { }

    public DidDereferencingException(string message, Exception inner) : base(message, inner) { }

    protected DidDereferencingException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}