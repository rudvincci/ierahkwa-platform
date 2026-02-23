using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.Accessibility.Domain.Exceptions;

internal class InvalidAccessProfileTagsException : DomainException
{
    public override string Code { get; } = "invalid_accessprofile_tags";

    public InvalidAccessProfileTagsException() : base("AccessProfile tags are invalid.")
    {
    }
}
