using System;
using Mamey.CQRS.Queries;
using Pupitre.Operations.Application.DTO;

namespace Pupitre.Operations.Application.Queries;

internal record GetOperationMetric(Guid Id) : IQuery<OperationMetricDetailsDto>;
