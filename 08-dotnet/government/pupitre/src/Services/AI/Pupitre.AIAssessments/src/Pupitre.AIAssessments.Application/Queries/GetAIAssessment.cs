using System;
using Mamey.CQRS.Queries;
using Pupitre.AIAssessments.Application.DTO;

namespace Pupitre.AIAssessments.Application.Queries;

internal record GetAIAssessment(Guid Id) : IQuery<AIAssessmentDetailsDto>;
