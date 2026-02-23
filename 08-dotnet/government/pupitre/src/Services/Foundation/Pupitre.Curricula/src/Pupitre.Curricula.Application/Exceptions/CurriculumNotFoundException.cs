using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Curricula.Domain.Entities;

namespace Pupitre.Curricula.Application.Exceptions;

internal class CurriculumNotFoundException : MameyException
{
    public CurriculumNotFoundException(CurriculumId curriculumId)
        : base($"Curriculum with ID: '{curriculumId.Value}' was not found.")
        => CurriculumId = curriculumId;

    public CurriculumId CurriculumId { get; }
}

