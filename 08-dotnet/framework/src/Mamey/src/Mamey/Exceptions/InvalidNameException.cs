namespace Mamey.Exceptions;

public class InvalidNameException : DomainException
{
    public InvalidNameException()
        : base("Name is invalid.")
    {
    }
    public override string Code => "invalid_name";
}