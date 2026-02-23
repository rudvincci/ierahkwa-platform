using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingPermissionResourceException : DomainException
{
    public override string Code { get; } = "missing_permission_resource";

    public MissingPermissionResourceException() : base("Permission resource is missing.")
    {
    }
}
