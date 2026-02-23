using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Educators.Domain.Entities;

namespace Pupitre.Educators.Application.Exceptions;

internal class EducatorAlreadyExistsException : MameyException
{
    public EducatorAlreadyExistsException(EducatorId educatorId)
        : base($"Educator with ID: '{educatorId.Value}' already exists.")
        => EducatorId = educatorId;

    public EducatorId EducatorId { get; }
}
