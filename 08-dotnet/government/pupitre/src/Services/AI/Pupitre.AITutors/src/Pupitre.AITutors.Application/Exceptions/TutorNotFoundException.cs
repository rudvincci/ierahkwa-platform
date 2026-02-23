using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AITutors.Domain.Entities;

namespace Pupitre.AITutors.Application.Exceptions;

internal class TutorNotFoundException : MameyException
{
    public TutorNotFoundException(TutorId tutorId)
        : base($"Tutor with ID: '{tutorId.Value}' was not found.")
        => TutorId = tutorId;

    public TutorId TutorId { get; }
}

