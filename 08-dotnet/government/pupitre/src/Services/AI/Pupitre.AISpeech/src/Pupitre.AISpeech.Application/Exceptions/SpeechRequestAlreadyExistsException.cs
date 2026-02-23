using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AISpeech.Domain.Entities;

namespace Pupitre.AISpeech.Application.Exceptions;

internal class SpeechRequestAlreadyExistsException : MameyException
{
    public SpeechRequestAlreadyExistsException(SpeechRequestId speechrequestId)
        : base($"SpeechRequest with ID: '{speechrequestId.Value}' already exists.")
        => SpeechRequestId = speechrequestId;

    public SpeechRequestId SpeechRequestId { get; }
}
