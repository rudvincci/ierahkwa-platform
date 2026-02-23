using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.Educators.Domain.Exceptions;

internal class InvalidEducatorTagsException : DomainException
{
    public override string Code { get; } = "invalid_educator_tags";

    public InvalidEducatorTagsException() : base("Educator tags are invalid.")
    {
    }
}
