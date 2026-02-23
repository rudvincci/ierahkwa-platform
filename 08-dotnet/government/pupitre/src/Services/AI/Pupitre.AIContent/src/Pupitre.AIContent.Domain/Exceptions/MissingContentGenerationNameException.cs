using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.AIContent.Domain.Exceptions;

internal class MissingContentGenerationNameException : DomainException
{
    public MissingContentGenerationNameException()
        : base("ContentGeneration name is missing.")
    {
    }

    public override string Code => "missing_contentgeneration_name";
}
