using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AIAdaptive.Domain.Entities;

namespace Pupitre.AIAdaptive.Application.Exceptions;

internal class AdaptiveLearningNotFoundException : MameyException
{
    public AdaptiveLearningNotFoundException(AdaptiveLearningId adaptivelearningId)
        : base($"AdaptiveLearning with ID: '{adaptivelearningId.Value}' was not found.")
        => AdaptiveLearningId = adaptivelearningId;

    public AdaptiveLearningId AdaptiveLearningId { get; }
}

