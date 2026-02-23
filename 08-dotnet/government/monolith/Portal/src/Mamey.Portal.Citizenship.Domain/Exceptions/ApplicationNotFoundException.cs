namespace Mamey.Portal.Citizenship.Domain.Exceptions;

public sealed class ApplicationNotFoundException : Exception
{
    public ApplicationNotFoundException(Guid applicationId)
        : base($"Citizenship application '{applicationId}' was not found.")
    {
    }
}
