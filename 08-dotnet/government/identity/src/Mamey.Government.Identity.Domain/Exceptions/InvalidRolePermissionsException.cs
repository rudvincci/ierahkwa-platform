using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class InvalidRolePermissionsException : DomainException
{
    public override string Code { get; } = "invalid_role_permissions";

    public InvalidRolePermissionsException() : base("Role permissions are invalid.")
    {
    }

    public InvalidRolePermissionsException(RoleId roleId) 
        : base($"Role permissions are invalid for role '{roleId}'.")
    {
    }
}
