using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class RoleAlreadyInactiveException : DomainException
{
    public override string Code { get; } = "role_already_inactive";

    public RoleAlreadyInactiveException() : base("Role is already inactive.")
    {
    }
}
