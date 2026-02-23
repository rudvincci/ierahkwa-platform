using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AISpeech.Domain.Entities;

namespace Pupitre.AISpeech.Application.Exceptions;

internal class SpeechRequestNotFoundException : MameyException
{
    public SpeechRequestNotFoundException(SpeechRequestId speechrequestId)
        : base($"SpeechRequest with ID: '{speechrequestId.Value}' was not found.")
        => SpeechRequestId = speechrequestId;

    public SpeechRequestId SpeechRequestId { get; }
}

