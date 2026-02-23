using Mamey.Exceptions;

namespace Pupitre.Analytics.Domain.Exceptions;

internal class MissingAnalyticTagsException : DomainException
{
    public MissingAnalyticTagsException()
        : base("Analytic tags are missing.")
    {
    }

    public override string Code => "missing_analytic_tags";
}