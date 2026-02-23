using System;
using Mamey.CQRS.Queries;
using Pupitre.AITutors.Application.DTO;

namespace Pupitre.AITutors.Application.Queries;

internal record GetTutor(Guid Id) : IQuery<TutorDetailsDto>;
