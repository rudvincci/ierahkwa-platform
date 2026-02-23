using Mamey.Exceptions;

namespace Pupitre.AIContent.Domain.Exceptions;

internal class MissingContentGenerationTagsException : DomainException
{
    public MissingContentGenerationTagsException()
        : base("ContentGeneration tags are missing.")
    {
    }

    public override string Code => "missing_contentgeneration_tags";
}