using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.AISpeech.Domain.Exceptions;

internal class MissingSpeechRequestNameException : DomainException
{
    public MissingSpeechRequestNameException()
        : base("SpeechRequest name is missing.")
    {
    }

    public override string Code => "missing_speechrequest_name";
}
