using Mamey.Exceptions;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class PermissionAlreadyActiveException : DomainException
{
    public override string Code { get; } = "permission_already_active";

    public PermissionAlreadyActiveException() : base("Permission is already active.")
    {
    }

    public PermissionAlreadyActiveException(PermissionId permissionId) 
        : base($"Permission '{permissionId}' is already active.")
    {
    }
}
