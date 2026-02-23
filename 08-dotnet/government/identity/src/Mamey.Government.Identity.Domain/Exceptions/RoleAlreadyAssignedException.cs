using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class RoleAlreadyAssignedException : DomainException
{
    public override string Code { get; } = "role_already_assigned";

    public RoleAlreadyAssignedException() : base("Role is already assigned to the subject.")
    {
    }
}
