using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class InvalidSubjectRolesException : DomainException
{
    public override string Code { get; } = "invalid_subject_roles";

    public InvalidSubjectRolesException() : base("Subject roles are invalid.")
    {
    }
}
