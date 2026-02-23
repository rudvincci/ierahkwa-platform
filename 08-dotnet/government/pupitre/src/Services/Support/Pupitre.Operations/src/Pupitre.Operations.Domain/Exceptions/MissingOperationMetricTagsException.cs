using Mamey.Exceptions;

namespace Pupitre.Operations.Domain.Exceptions;

internal class MissingOperationMetricTagsException : DomainException
{
    public MissingOperationMetricTagsException()
        : base("OperationMetric tags are missing.")
    {
    }

    public override string Code => "missing_operationmetric_tags";
}