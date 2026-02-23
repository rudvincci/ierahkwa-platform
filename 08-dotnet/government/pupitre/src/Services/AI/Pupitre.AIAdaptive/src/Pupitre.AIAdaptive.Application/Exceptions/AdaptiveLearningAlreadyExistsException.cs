using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AIAdaptive.Domain.Entities;

namespace Pupitre.AIAdaptive.Application.Exceptions;

internal class AdaptiveLearningAlreadyExistsException : MameyException
{
    public AdaptiveLearningAlreadyExistsException(AdaptiveLearningId adaptivelearningId)
        : base($"AdaptiveLearning with ID: '{adaptivelearningId.Value}' already exists.")
        => AdaptiveLearningId = adaptivelearningId;

    public AdaptiveLearningId AdaptiveLearningId { get; }
}
