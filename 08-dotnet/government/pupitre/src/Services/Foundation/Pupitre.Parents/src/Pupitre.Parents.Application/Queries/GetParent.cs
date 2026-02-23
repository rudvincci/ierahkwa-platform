using System;
using Mamey.CQRS.Queries;
using Pupitre.Parents.Application.DTO;

namespace Pupitre.Parents.Application.Queries;

internal record GetParent(Guid Id) : IQuery<ParentDetailsDto>;
