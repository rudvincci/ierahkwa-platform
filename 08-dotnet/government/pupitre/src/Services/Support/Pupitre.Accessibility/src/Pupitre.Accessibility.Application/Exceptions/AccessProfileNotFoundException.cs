using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Accessibility.Domain.Entities;

namespace Pupitre.Accessibility.Application.Exceptions;

internal class AccessProfileNotFoundException : MameyException
{
    public AccessProfileNotFoundException(AccessProfileId accessprofileId)
        : base($"AccessProfile with ID: '{accessprofileId.Value}' was not found.")
        => AccessProfileId = accessprofileId;

    public AccessProfileId AccessProfileId { get; }
}

