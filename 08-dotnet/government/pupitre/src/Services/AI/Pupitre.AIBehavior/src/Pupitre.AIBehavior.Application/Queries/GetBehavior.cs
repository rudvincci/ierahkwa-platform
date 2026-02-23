using System;
using Mamey.CQRS.Queries;
using Pupitre.AIBehavior.Application.DTO;

namespace Pupitre.AIBehavior.Application.Queries;

internal record GetBehavior(Guid Id) : IQuery<BehaviorDetailsDto>;
