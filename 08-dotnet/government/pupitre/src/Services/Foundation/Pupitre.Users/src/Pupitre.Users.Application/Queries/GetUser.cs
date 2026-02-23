using System;
using Mamey.CQRS.Queries;
using Pupitre.Users.Application.DTO;

namespace Pupitre.Users.Application.Queries;

internal record GetUser(Guid Id) : IQuery<UserDetailsDto>;
