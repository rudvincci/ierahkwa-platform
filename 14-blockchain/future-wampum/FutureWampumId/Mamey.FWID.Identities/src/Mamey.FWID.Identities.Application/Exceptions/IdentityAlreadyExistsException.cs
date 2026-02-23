using Mamey.Exceptions;
using Mamey.Types;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Exceptions;

internal class IdentityAlreadyExistsException : MameyException
{
    public IdentityAlreadyExistsException(IdentityId identityId)
        : base($"Identity with ID: '{identityId.Value}' already exists.")
        => IdentityId = identityId;

    public IdentityId IdentityId { get; }
}
