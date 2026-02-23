using System;
using Mamey.CQRS.Queries;
using Pupitre.Educators.Application.DTO;

namespace Pupitre.Educators.Application.Queries;

internal record GetEducator(Guid Id) : IQuery<EducatorDetailsDto>;
