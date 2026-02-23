using Mamey.Exceptions;
using Mamey.Government.Identity.Domain.Exceptions;

namespace Mamey.Government.Identity.Application.Exceptions;

public class PermissionNotFoundException : DomainException
{
    public PermissionNotFoundException(Guid permissionId) : base($"Permission with ID '{permissionId}' was not found.")
    {
    }
}
