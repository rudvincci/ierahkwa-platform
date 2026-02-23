using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingPermissionDescriptionException : DomainException
{
    public override string Code { get; } = "missing_permission_description";

    public MissingPermissionDescriptionException() : base("Permission description is missing.")
    {
    }
}
