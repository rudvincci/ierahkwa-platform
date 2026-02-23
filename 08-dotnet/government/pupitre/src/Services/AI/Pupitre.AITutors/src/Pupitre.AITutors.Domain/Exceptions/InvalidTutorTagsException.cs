using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.AITutors.Domain.Exceptions;

internal class InvalidTutorTagsException : DomainException
{
    public override string Code { get; } = "invalid_tutor_tags";

    public InvalidTutorTagsException() : base("Tutor tags are invalid.")
    {
    }
}
