using Mamey.Exceptions;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class PermissionInUseException : DomainException
{
    public PermissionInUseException() : base("Permission is currently in use and cannot be deleted.")
    {
    }

    public PermissionInUseException(PermissionId permissionId) 
        : base($"Permission '{permissionId}' is currently in use and cannot be deleted.")
    {
    }
}
