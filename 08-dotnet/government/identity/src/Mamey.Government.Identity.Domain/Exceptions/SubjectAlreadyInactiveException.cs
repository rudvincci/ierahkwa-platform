using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class SubjectAlreadyInactiveException : DomainException
{
    public override string Code { get; } = "subject_already_inactive";

    public SubjectAlreadyInactiveException() : base("Subject is already inactive.")
    {
    }
}
