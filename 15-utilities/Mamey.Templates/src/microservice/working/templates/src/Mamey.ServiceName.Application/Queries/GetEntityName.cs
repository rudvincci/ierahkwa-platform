using System;
using Mamey.CQRS.Queries;
using Mamey.ServiceName.Application.DTO;

namespace Mamey.ServiceName.Application.Queries;

internal record GetEntityName(Guid Id) : IQuery<EntityNameDetailsDto>;
