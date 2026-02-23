using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Progress.Domain.Entities;

namespace Pupitre.Progress.Application.Exceptions;

internal class LearningProgressNotFoundException : MameyException
{
    public LearningProgressNotFoundException(LearningProgressId learningprogressId)
        : base($"LearningProgress with ID: '{learningprogressId.Value}' was not found.")
        => LearningProgressId = learningprogressId;

    public LearningProgressId LearningProgressId { get; }
}

