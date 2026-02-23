using Mamey.Exceptions;

namespace Pupitre.AITutors.Domain.Exceptions;

internal class MissingTutorTagsException : DomainException
{
    public MissingTutorTagsException()
        : base("Tutor tags are missing.")
    {
    }

    public override string Code => "missing_tutor_tags";
}