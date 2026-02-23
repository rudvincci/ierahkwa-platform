namespace Mamey.Exceptions;

public class MissingOrganizationHeaderException : MameyException
{

    public MissingOrganizationHeaderException()
        : base("Missing Organization header 'X-ORG'.")
    {
    }
}

