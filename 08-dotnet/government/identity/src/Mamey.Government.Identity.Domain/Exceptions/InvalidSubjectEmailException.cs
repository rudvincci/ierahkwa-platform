using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class InvalidSubjectEmailException : DomainException
{
    public override string Code { get; } = "invalid_subject_email";

    public InvalidSubjectEmailException() : base("Subject email is invalid.")
    {
    }
}
