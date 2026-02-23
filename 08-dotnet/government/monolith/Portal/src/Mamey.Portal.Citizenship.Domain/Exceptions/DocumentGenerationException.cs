namespace Mamey.Portal.Citizenship.Domain.Exceptions;

public sealed class DocumentGenerationException : Exception
{
    public DocumentGenerationException(string message)
        : base(message)
    {
    }
}
