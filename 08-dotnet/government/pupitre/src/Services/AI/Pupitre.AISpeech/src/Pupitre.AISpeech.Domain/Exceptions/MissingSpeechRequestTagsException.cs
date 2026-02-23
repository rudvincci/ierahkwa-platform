using Mamey.Exceptions;

namespace Pupitre.AISpeech.Domain.Exceptions;

internal class MissingSpeechRequestTagsException : DomainException
{
    public MissingSpeechRequestTagsException()
        : base("SpeechRequest tags are missing.")
    {
    }

    public override string Code => "missing_speechrequest_tags";
}