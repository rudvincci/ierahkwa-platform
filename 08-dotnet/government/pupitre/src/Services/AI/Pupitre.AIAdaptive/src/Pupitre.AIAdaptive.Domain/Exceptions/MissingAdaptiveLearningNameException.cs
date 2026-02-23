using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.AIAdaptive.Domain.Exceptions;

internal class MissingAdaptiveLearningNameException : DomainException
{
    public MissingAdaptiveLearningNameException()
        : base("AdaptiveLearning name is missing.")
    {
    }

    public override string Code => "missing_adaptivelearning_name";
}
