using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Accessibility.Domain.Entities;

namespace Pupitre.Accessibility.Application.Exceptions;

internal class AccessProfileAlreadyExistsException : MameyException
{
    public AccessProfileAlreadyExistsException(AccessProfileId accessprofileId)
        : base($"AccessProfile with ID: '{accessprofileId.Value}' already exists.")
        => AccessProfileId = accessprofileId;

    public AccessProfileId AccessProfileId { get; }
}
