using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Progress.Domain.Entities;

namespace Pupitre.Progress.Application.Exceptions;

internal class LearningProgressAlreadyExistsException : MameyException
{
    public LearningProgressAlreadyExistsException(LearningProgressId learningprogressId)
        : base($"LearningProgress with ID: '{learningprogressId.Value}' already exists.")
        => LearningProgressId = learningprogressId;

    public LearningProgressId LearningProgressId { get; }
}
