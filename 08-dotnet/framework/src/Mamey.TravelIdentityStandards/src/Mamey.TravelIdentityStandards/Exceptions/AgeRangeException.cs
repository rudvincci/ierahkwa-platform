using Mamey.Exceptions;

namespace Mamey.TravelIdentityStandards.Exceptions;

public class AgeRangeException : MameyException
{
    public AgeRangeException(string message) : base(message)
    {
    }
    public AgeRangeException(int age) : base($"Age of {age}, does not meet age requirements.")
    {
        Age = age;
    }
    
    public int Age { get; }
}