using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AITutors.Domain.Entities;

namespace Pupitre.AITutors.Application.Exceptions;

internal class TutorAlreadyExistsException : MameyException
{
    public TutorAlreadyExistsException(TutorId tutorId)
        : base($"Tutor with ID: '{tutorId.Value}' already exists.")
        => TutorId = tutorId;

    public TutorId TutorId { get; }
}
