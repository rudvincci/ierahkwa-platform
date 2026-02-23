using System;
using Mamey.CQRS.Queries;
using Pupitre.Ministries.Application.DTO;

namespace Pupitre.Ministries.Application.Queries;

internal record GetMinistryData(Guid Id) : IQuery<MinistryDataDetailsDto>;
