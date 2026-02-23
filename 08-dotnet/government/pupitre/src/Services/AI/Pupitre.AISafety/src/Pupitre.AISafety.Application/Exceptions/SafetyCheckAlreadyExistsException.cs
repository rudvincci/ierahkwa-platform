using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AISafety.Domain.Entities;

namespace Pupitre.AISafety.Application.Exceptions;

internal class SafetyCheckAlreadyExistsException : MameyException
{
    public SafetyCheckAlreadyExistsException(SafetyCheckId safetycheckId)
        : base($"SafetyCheck with ID: '{safetycheckId.Value}' already exists.")
        => SafetyCheckId = safetycheckId;

    public SafetyCheckId SafetyCheckId { get; }
}
