using System;
using Mamey.CQRS.Queries;
using Pupitre.Assessments.Application.DTO;

namespace Pupitre.Assessments.Application.Queries;

internal record GetAssessment(Guid Id) : IQuery<AssessmentDetailsDto>;
