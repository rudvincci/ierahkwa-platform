using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

public class RoleInUseException : DomainException
{
    public RoleInUseException() : base("Role is currently in use and cannot be deleted.")
    {
    }

    public RoleInUseException(RoleId roleId) 
        : base($"Role '{roleId}' is currently in use and cannot be deleted.")
    {
    }
}
