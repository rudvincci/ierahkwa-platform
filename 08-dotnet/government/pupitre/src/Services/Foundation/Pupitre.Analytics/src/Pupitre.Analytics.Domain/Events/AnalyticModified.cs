using Mamey.CQRS;
using Pupitre.Analytics.Domain.Entities;

namespace Pupitre.Analytics.Domain.Events;

internal record AnalyticModified(Analytic Analytic): IDomainEvent;

