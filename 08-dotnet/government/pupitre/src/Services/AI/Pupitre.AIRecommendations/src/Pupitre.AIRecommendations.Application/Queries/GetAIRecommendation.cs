using System;
using Mamey.CQRS.Queries;
using Pupitre.AIRecommendations.Application.DTO;

namespace Pupitre.AIRecommendations.Application.Queries;

internal record GetAIRecommendation(Guid Id) : IQuery<AIRecommendationDetailsDto>;
