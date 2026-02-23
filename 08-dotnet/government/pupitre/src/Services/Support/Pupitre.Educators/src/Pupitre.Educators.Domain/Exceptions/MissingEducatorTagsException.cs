using Mamey.Exceptions;

namespace Pupitre.Educators.Domain.Exceptions;

internal class MissingEducatorTagsException : DomainException
{
    public MissingEducatorTagsException()
        : base("Educator tags are missing.")
    {
    }

    public override string Code => "missing_educator_tags";
}