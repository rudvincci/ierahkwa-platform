using System;
using Mamey.Exceptions;

namespace Mamey.Government.Modules.Citizens.Core.Exceptions;

public sealed class CitizenNotFoundException : MameyException
{
    public Guid CitizenId { get; }

    public CitizenNotFoundException(Guid citizenId)
        : base($"Citizen with ID '{citizenId}' was not found.")
    {
        CitizenId = citizenId;
    }
}
