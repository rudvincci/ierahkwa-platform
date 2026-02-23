using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

public class RoleNotFoundException : DomainException
{
    public RoleNotFoundException() : base("Role not found.")
    {
    }

    public RoleNotFoundException(RoleId roleId) 
        : base($"Role '{roleId}' not found.")
    {
    }
}
