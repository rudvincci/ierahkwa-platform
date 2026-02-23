using Mamey.Exceptions;

namespace Pupitre.Parents.Domain.Exceptions;

internal class MissingParentTagsException : DomainException
{
    public MissingParentTagsException()
        : base("Parent tags are missing.")
    {
    }

    public override string Code => "missing_parent_tags";
}