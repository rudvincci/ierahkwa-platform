using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingPermissionNameException : DomainException
{
    public override string Code { get; } = "missing_permission_name";

    public MissingPermissionNameException() : base("Permission name is missing.")
    {
    }
}
