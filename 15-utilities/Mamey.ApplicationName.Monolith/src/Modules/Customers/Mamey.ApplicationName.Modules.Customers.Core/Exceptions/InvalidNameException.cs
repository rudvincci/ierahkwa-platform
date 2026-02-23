using Mamey.Exceptions;

namespace Mamey.ApplicationName.Modules.Customers.Core.Exceptions;
internal class InvalidNameException : MameyException
{
    public string Name { get; }

    public InvalidNameException(string name) : base($"Name: '{name}' is invalid.")
    {
        Name = name;
    }
}
