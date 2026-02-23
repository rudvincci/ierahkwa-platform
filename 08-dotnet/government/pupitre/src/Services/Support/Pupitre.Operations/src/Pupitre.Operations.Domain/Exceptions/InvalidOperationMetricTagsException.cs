using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.Operations.Domain.Exceptions;

internal class InvalidOperationMetricTagsException : DomainException
{
    public override string Code { get; } = "invalid_operationmetric_tags";

    public InvalidOperationMetricTagsException() : base("OperationMetric tags are invalid.")
    {
    }
}
