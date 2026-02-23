using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.Analytics.Domain.Exceptions;

internal class InvalidAnalyticTagsException : DomainException
{
    public override string Code { get; } = "invalid_analytic_tags";

    public InvalidAnalyticTagsException() : base("Analytic tags are invalid.")
    {
    }
}
