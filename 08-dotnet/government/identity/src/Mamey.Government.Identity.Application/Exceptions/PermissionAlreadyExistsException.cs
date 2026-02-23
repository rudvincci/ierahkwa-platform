using Mamey.Exceptions;
using Mamey.Government.Identity.Domain.Exceptions;

namespace Mamey.Government.Identity.Application.Exceptions;

public class PermissionAlreadyExistsException : DomainException
{
    public PermissionAlreadyExistsException(Guid permissionId) : base($"Permission with ID '{permissionId}' already exists.")
    {
    }
}
