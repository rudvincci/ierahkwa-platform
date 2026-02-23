using Mamey.Exceptions;

namespace Mamey.ApplicationName.Modules.Customers.Core.Exceptions;

internal class UserNotFoundException : MameyException
{
    public string Email { get; }
        
    public UserNotFoundException(string email) : base($"User with email: '{email}' was not found.")
    {
        Email = email;
    }
}