using Mamey.Exceptions;
using Mamey.Government.Identity.Domain.Entities;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class PermissionNotFoundException : DomainException
{
    public PermissionNotFoundException() : base("Permission not found.")
    {
    }

    public PermissionNotFoundException(PermissionId permissionId) 
        : base($"Permission '{permissionId}' not found.")
    {
    }
}
