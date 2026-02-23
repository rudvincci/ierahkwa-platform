using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.Operations.Domain.Exceptions;

internal class MissingOperationMetricNameException : DomainException
{
    public MissingOperationMetricNameException()
        : base("OperationMetric name is missing.")
    {
    }

    public override string Code => "missing_operationmetric_name";
}
