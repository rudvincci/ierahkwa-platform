using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Analytics.Domain.Entities;

namespace Pupitre.Analytics.Application.Exceptions;

internal class AnalyticNotFoundException : MameyException
{
    public AnalyticNotFoundException(AnalyticId analyticId)
        : base($"Analytic with ID: '{analyticId.Value}' was not found.")
        => AnalyticId = analyticId;

    public AnalyticId AnalyticId { get; }
}

