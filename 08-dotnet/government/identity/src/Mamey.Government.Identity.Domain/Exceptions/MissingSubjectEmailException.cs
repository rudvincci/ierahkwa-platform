using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingSubjectEmailException : DomainException
{
    public override string Code { get; } = "missing_subject_email";

    public MissingSubjectEmailException() : base("Subject email is missing.")
    {
    }
}
