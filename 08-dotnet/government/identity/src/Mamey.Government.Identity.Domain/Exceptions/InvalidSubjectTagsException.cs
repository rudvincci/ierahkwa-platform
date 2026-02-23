using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class InvalidSubjectTagsException : DomainException
{
    public override string Code { get; } = "invalid_subject_tags";

    public InvalidSubjectTagsException() : base("Subject tags are invalid.")
    {
    }
}
