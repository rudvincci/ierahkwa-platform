using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingRoleNameException : DomainException
{
    public override string Code { get; } = "missing_role_name";

    public MissingRoleNameException() : base("Role name is missing.")
    {
    }
}
