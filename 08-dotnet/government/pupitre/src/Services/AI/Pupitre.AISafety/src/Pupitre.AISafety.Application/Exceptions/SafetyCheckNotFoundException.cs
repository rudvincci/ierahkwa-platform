using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AISafety.Domain.Entities;

namespace Pupitre.AISafety.Application.Exceptions;

internal class SafetyCheckNotFoundException : MameyException
{
    public SafetyCheckNotFoundException(SafetyCheckId safetycheckId)
        : base($"SafetyCheck with ID: '{safetycheckId.Value}' was not found.")
        => SafetyCheckId = safetycheckId;

    public SafetyCheckId SafetyCheckId { get; }
}

