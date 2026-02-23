using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.AITranslation.Domain.Exceptions;

internal class InvalidTranslationRequestTagsException : DomainException
{
    public override string Code { get; } = "invalid_translationrequest_tags";

    public InvalidTranslationRequestTagsException() : base("TranslationRequest tags are invalid.")
    {
    }
}
