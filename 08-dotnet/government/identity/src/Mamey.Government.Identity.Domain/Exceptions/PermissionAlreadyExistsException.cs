using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class PermissionAlreadyExistsException : DomainException
{
    public override string Code { get; } = "permission_already_exists";

    public PermissionAlreadyExistsException() : base("Permission already exists.")
    {
    }

    public PermissionAlreadyExistsException(string name, string resource) 
        : base($"Permission with name '{name}' and resource '{resource}' already exists.")
    {
    }
}
