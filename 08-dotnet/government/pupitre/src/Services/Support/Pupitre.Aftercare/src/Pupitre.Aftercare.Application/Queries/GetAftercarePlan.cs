using System;
using Mamey.CQRS.Queries;
using Pupitre.Aftercare.Application.DTO;

namespace Pupitre.Aftercare.Application.Queries;

internal record GetAftercarePlan(Guid Id) : IQuery<AftercarePlanDetailsDto>;
