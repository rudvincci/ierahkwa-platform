using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingRoleDescriptionException : DomainException
{
    public override string Code { get; } = "missing_role_description";

    public MissingRoleDescriptionException() : base("Role description is missing.")
    {
    }
}
