using System;
using Mamey.Exceptions;
using Mamey.MicroMonolith.Abstractions.Exceptions;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Exceptions;

public sealed class ApplicationNotFoundException : MameyException
{
    public Guid ApplicationId { get; }

    public ApplicationNotFoundException(Guid applicationId)
        : base($"Citizenship application with ID '{applicationId}' was not found.")
    {
        ApplicationId = applicationId;
    }
}
