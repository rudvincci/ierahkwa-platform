using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.AIAssessments.Domain.Exceptions;

internal class MissingAIAssessmentNameException : DomainException
{
    public MissingAIAssessmentNameException()
        : base("AIAssessment name is missing.")
    {
    }

    public override string Code => "missing_aiassessment_name";
}
