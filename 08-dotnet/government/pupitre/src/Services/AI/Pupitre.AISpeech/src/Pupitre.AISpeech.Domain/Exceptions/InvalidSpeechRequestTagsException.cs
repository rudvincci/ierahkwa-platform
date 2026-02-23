using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.AISpeech.Domain.Exceptions;

internal class InvalidSpeechRequestTagsException : DomainException
{
    public override string Code { get; } = "invalid_speechrequest_tags";

    public InvalidSpeechRequestTagsException() : base("SpeechRequest tags are invalid.")
    {
    }
}
