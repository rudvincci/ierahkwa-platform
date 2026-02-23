namespace Mamey.Exceptions;

public class InvalidOrganizationException : MameyException
{
    public InvalidOrganizationException() : base($"X-ORG header is not defined)")
    { }
}
