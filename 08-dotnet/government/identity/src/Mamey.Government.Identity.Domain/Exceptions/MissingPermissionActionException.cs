using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingPermissionActionException : DomainException
{
    public override string Code { get; } = "missing_permission_action";

    public MissingPermissionActionException() : base("Permission action is missing.")
    {
    }
}
