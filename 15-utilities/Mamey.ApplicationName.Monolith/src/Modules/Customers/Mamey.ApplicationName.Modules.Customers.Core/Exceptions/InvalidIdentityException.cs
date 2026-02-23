using Mamey.Exceptions;

namespace Mamey.ApplicationName.Modules.Customers.Core.Exceptions;
internal class InvalidIdentityException : MameyException
{
    public string Type { get; }

    public InvalidIdentityException(string type, string series)
        : base($"Identity type: '{type}', series: '{series}' is invalid.")
    {
        Type = type;
    }
}
