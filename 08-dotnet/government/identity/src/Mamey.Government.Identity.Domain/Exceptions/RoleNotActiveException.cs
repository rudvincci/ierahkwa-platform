using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

public class RoleNotActiveException : DomainException
{
    public RoleNotActiveException() : base("Role is not active.")
    {
    }

    public RoleNotActiveException(RoleId roleId) 
        : base($"Role '{roleId}' is not active.")
    {
    }
}
