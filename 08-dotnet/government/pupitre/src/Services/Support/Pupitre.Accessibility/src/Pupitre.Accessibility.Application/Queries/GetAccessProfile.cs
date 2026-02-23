using System;
using Mamey.CQRS.Queries;
using Pupitre.Accessibility.Application.DTO;

namespace Pupitre.Accessibility.Application.Queries;

internal record GetAccessProfile(Guid Id) : IQuery<AccessProfileDetailsDto>;
