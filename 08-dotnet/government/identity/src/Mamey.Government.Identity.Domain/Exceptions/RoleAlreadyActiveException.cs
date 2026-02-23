using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class RoleAlreadyActiveException : DomainException
{
    public override string Code { get; } = "role_already_active";

    public RoleAlreadyActiveException() : base("Role is already active.")
    {
    }
}
