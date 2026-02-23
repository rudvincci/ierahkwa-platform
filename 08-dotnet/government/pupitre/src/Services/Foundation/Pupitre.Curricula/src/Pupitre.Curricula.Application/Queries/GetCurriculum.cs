using System;
using Mamey.CQRS.Queries;
using Pupitre.Curricula.Application.DTO;

namespace Pupitre.Curricula.Application.Queries;

internal record GetCurriculum(Guid Id) : IQuery<CurriculumDetailsDto>;
