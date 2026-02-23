using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Curricula.Domain.Entities;

namespace Pupitre.Curricula.Application.Exceptions;

internal class CurriculumAlreadyExistsException : MameyException
{
    public CurriculumAlreadyExistsException(CurriculumId curriculumId)
        : base($"Curriculum with ID: '{curriculumId.Value}' already exists.")
        => CurriculumId = curriculumId;

    public CurriculumId CurriculumId { get; }
}
