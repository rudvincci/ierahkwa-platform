using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Operations.Domain.Entities;

namespace Pupitre.Operations.Application.Exceptions;

internal class OperationMetricAlreadyExistsException : MameyException
{
    public OperationMetricAlreadyExistsException(OperationMetricId operationmetricId)
        : base($"OperationMetric with ID: '{operationmetricId.Value}' already exists.")
        => OperationMetricId = operationmetricId;

    public OperationMetricId OperationMetricId { get; }
}
