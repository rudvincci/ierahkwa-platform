using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AITranslation.Domain.Entities;

namespace Pupitre.AITranslation.Application.Exceptions;

internal class TranslationRequestNotFoundException : MameyException
{
    public TranslationRequestNotFoundException(TranslationRequestId translationrequestId)
        : base($"TranslationRequest with ID: '{translationrequestId.Value}' was not found.")
        => TranslationRequestId = translationrequestId;

    public TranslationRequestId TranslationRequestId { get; }
}

