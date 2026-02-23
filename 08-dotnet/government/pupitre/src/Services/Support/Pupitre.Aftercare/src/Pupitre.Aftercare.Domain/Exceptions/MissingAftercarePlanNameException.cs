using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.Aftercare.Domain.Exceptions;

internal class MissingAftercarePlanNameException : DomainException
{
    public MissingAftercarePlanNameException()
        : base("AftercarePlan name is missing.")
    {
    }

    public override string Code => "missing_aftercareplan_name";
}
