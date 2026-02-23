using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.Assessments.Domain.Exceptions;

internal class MissingAssessmentNameException : DomainException
{
    public MissingAssessmentNameException()
        : base("Assessment name is missing.")
    {
    }

    public override string Code => "missing_assessment_name";
}
