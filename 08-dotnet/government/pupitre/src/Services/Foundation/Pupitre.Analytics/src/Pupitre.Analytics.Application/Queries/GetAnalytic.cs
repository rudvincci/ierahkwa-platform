using System;
using Mamey.CQRS.Queries;
using Pupitre.Analytics.Application.DTO;

namespace Pupitre.Analytics.Application.Queries;

internal record GetAnalytic(Guid Id) : IQuery<AnalyticDetailsDto>;
