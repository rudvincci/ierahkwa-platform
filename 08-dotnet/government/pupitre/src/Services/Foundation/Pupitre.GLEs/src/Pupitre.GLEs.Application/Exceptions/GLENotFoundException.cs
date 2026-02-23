using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.GLEs.Domain.Entities;

namespace Pupitre.GLEs.Application.Exceptions;

internal class GLENotFoundException : MameyException
{
    public GLENotFoundException(GLEId gleId)
        : base($"GLE with ID: '{gleId.Value}' was not found.")
        => GLEId = gleId;

    public GLEId GLEId { get; }
}

