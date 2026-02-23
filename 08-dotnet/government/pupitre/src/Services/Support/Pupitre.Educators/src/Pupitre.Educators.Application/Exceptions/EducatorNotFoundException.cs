using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Educators.Domain.Entities;

namespace Pupitre.Educators.Application.Exceptions;

internal class EducatorNotFoundException : MameyException
{
    public EducatorNotFoundException(EducatorId educatorId)
        : base($"Educator with ID: '{educatorId.Value}' was not found.")
        => EducatorId = educatorId;

    public EducatorId EducatorId { get; }
}

