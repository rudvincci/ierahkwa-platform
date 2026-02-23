using System.Runtime.Serialization;

namespace Mamey.Identity.Decentralized.Exceptions;

/// <summary>
/// Represents errors encountered during DID resolution.
/// </summary>
[Serializable]
public class DidResolutionException : Exception
{
    public DidResolutionException() { }

    public DidResolutionException(string message) : base(message) { }

    public DidResolutionException(string message, Exception inner) : base(message, inner) { }

    protected DidResolutionException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}