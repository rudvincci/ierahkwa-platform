using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class RoleNotAssignedException : DomainException
{
    public override string Code { get; } = "role_not_assigned";

    public RoleNotAssignedException() : base("Role is not assigned to the subject.")
    {
    }
}
