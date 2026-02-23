using Mamey.Exceptions;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class PermissionAlreadyInactiveException : DomainException
{
    public override string Code { get; } = "permission_already_inactive";

    public PermissionAlreadyInactiveException() : base("Permission is already inactive.")
    {
    }

    public PermissionAlreadyInactiveException(PermissionId permissionId) 
        : base($"Permission '{permissionId}' is already inactive.")
    {
    }
}
