using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AITranslation.Domain.Entities;

namespace Pupitre.AITranslation.Application.Exceptions;

internal class TranslationRequestAlreadyExistsException : MameyException
{
    public TranslationRequestAlreadyExistsException(TranslationRequestId translationrequestId)
        : base($"TranslationRequest with ID: '{translationrequestId.Value}' already exists.")
        => TranslationRequestId = translationrequestId;

    public TranslationRequestId TranslationRequestId { get; }
}
