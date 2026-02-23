using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class PermissionNotAssignedException : DomainException
{
    public override string Code { get; } = "permission_not_assigned";

    public PermissionNotAssignedException() : base("Permission is not assigned to the role.")
    {
    }
}
