using System;
using Mamey.CQRS.Queries;
using Pupitre.Progress.Application.DTO;

namespace Pupitre.Progress.Application.Queries;

internal record GetLearningProgress(Guid Id) : IQuery<LearningProgressDetailsDto>;
