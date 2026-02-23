using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class SubjectAlreadyActiveException : DomainException
{
    public override string Code { get; } = "subject_already_active";

    public SubjectAlreadyActiveException() : base("Subject is already active.")
    {
    }
}
