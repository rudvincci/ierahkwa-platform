using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.GLEs.Domain.Entities;

namespace Pupitre.GLEs.Application.Exceptions;

internal class GLEAlreadyExistsException : MameyException
{
    public GLEAlreadyExistsException(GLEId gleId)
        : base($"GLE with ID: '{gleId.Value}' already exists.")
        => GLEId = gleId;

    public GLEId GLEId { get; }
}
