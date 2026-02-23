using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.AIBehavior.Domain.Exceptions;

internal class MissingBehaviorNameException : DomainException
{
    public MissingBehaviorNameException()
        : base("Behavior name is missing.")
    {
    }

    public override string Code => "missing_behavior_name";
}
