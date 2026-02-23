using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Analytics.Domain.Entities;

namespace Pupitre.Analytics.Application.Exceptions;

internal class AnalyticAlreadyExistsException : MameyException
{
    public AnalyticAlreadyExistsException(AnalyticId analyticId)
        : base($"Analytic with ID: '{analyticId.Value}' already exists.")
        => AnalyticId = analyticId;

    public AnalyticId AnalyticId { get; }
}
