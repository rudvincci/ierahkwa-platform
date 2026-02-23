using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.AIContent.Domain.Exceptions;

internal class InvalidContentGenerationTagsException : DomainException
{
    public override string Code { get; } = "invalid_contentgeneration_tags";

    public InvalidContentGenerationTagsException() : base("ContentGeneration tags are invalid.")
    {
    }
}
