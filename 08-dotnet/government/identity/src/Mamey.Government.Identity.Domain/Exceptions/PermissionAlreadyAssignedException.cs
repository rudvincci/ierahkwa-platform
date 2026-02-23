using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class PermissionAlreadyAssignedException : DomainException
{
    public override string Code { get; } = "permission_already_assigned";

    public PermissionAlreadyAssignedException() : base("Permission is already assigned to the role.")
    {
    }
}
