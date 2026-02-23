using Mamey.Portal.Citizenship.Domain.ValueObjects;

namespace Mamey.Portal.Citizenship.Domain.Exceptions;

public sealed class InvalidApplicationStatusException : Exception
{
    public InvalidApplicationStatusException(Guid applicationId, ApplicationStatus current, ApplicationStatus target)
        : base($"Application '{applicationId}' cannot transition from '{current}' to '{target}'.")
    {
    }
}
