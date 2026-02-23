namespace Mamey.Exceptions;

public abstract class DomainException : MameyException
{
    protected DomainException()
        : base("Mamey Domain Exception thrown")
    {

    }
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string code, string reason) 
        : base("Mamey Domain Exception throw", code, reason)
    {
    }

    protected DomainException(string message, string code, string reason) 
        : base(message, code, reason)
    {
    }
}

