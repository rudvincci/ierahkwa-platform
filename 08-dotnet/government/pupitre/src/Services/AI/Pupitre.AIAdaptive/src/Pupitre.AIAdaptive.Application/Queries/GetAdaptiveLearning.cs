using System;
using Mamey.CQRS.Queries;
using Pupitre.AIAdaptive.Application.DTO;

namespace Pupitre.AIAdaptive.Application.Queries;

internal record GetAdaptiveLearning(Guid Id) : IQuery<AdaptiveLearningDetailsDto>;
