using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.Progress.Domain.Exceptions;

internal class MissingLearningProgressNameException : DomainException
{
    public MissingLearningProgressNameException()
        : base("LearningProgress name is missing.")
    {
    }

    public override string Code => "missing_learningprogress_name";
}
