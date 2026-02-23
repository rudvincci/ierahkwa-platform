using Mamey.Exceptions;
using Mamey.Types;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Exceptions;

internal class IdentityNotFoundException : MameyException
{
    public IdentityNotFoundException(IdentityId identityId)
        : base($"Identity with ID: '{identityId.Value}' was not found.")
        => IdentityId = identityId;

    public IdentityId IdentityId { get; }
}

