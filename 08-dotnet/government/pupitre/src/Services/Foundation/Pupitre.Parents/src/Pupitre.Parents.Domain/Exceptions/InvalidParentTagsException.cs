using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.Parents.Domain.Exceptions;

internal class InvalidParentTagsException : DomainException
{
    public override string Code { get; } = "invalid_parent_tags";

    public InvalidParentTagsException() : base("Parent tags are invalid.")
    {
    }
}
