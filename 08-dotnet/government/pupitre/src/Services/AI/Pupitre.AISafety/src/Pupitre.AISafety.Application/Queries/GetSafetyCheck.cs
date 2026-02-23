using System;
using Mamey.CQRS.Queries;
using Pupitre.AISafety.Application.DTO;

namespace Pupitre.AISafety.Application.Queries;

internal record GetSafetyCheck(Guid Id) : IQuery<SafetyCheckDetailsDto>;
