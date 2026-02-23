using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.Analytics.Domain.Exceptions;

internal class MissingAnalyticNameException : DomainException
{
    public MissingAnalyticNameException()
        : base("Analytic name is missing.")
    {
    }

    public override string Code => "missing_analytic_name";
}
