using Mamey.Exceptions;

namespace Pupitre.Accessibility.Domain.Exceptions;

internal class MissingAccessProfileTagsException : DomainException
{
    public MissingAccessProfileTagsException()
        : base("AccessProfile tags are missing.")
    {
    }

    public override string Code => "missing_accessprofile_tags";
}