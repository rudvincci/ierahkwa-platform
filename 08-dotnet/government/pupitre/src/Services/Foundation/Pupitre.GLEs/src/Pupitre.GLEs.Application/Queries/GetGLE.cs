using System;
using Mamey.CQRS.Queries;
using Pupitre.GLEs.Application.DTO;

namespace Pupitre.GLEs.Application.Queries;

internal record GetGLE(Guid Id) : IQuery<GLEDetailsDto>;
