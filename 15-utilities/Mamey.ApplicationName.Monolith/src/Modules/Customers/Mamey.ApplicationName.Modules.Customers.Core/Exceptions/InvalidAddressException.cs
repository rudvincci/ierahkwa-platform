using Mamey.Exceptions;

namespace Mamey.ApplicationName.Modules.Customers.Core.Exceptions;
internal class InvalidAddressException : MameyException
{
    public string Address { get; }

    public InvalidAddressException(string address) : base($"Address: '{address}' is invalid.")
    {
        Address = address;
    }
}
