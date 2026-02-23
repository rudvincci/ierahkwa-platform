using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Exceptions;

/// <summary>
/// Exception thrown when an identity is not active.
/// </summary>
public class IdentityNotActiveException : Exception
{
    public IdentityId IdentityId { get; }

    public IdentityNotActiveException(IdentityId identityId) 
        : base($"Identity {identityId.Value} is not active")
    {
        IdentityId = identityId;
    }

    public IdentityNotActiveException(IdentityId identityId, string message) 
        : base(message)
    {
        IdentityId = identityId;
    }
}
