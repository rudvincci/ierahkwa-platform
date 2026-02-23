using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingSubjectTagsException : DomainException
{
    public MissingSubjectTagsException()
        : base("Subject tags are missing.")
    {
    }

    public override string Code => "missing_subject_tags";
}