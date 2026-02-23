using Mamey.Exceptions;

namespace Pupitre.AITranslation.Domain.Exceptions;

internal class MissingTranslationRequestTagsException : DomainException
{
    public MissingTranslationRequestTagsException()
        : base("TranslationRequest tags are missing.")
    {
    }

    public override string Code => "missing_translationrequest_tags";
}