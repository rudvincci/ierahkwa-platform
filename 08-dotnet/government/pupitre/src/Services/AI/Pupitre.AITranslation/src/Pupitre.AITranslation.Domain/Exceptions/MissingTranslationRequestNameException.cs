using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.AITranslation.Domain.Exceptions;

internal class MissingTranslationRequestNameException : DomainException
{
    public MissingTranslationRequestNameException()
        : base("TranslationRequest name is missing.")
    {
    }

    public override string Code => "missing_translationrequest_name";
}
