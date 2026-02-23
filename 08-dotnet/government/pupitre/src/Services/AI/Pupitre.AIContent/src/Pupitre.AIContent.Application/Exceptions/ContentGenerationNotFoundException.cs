using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AIContent.Domain.Entities;

namespace Pupitre.AIContent.Application.Exceptions;

internal class ContentGenerationNotFoundException : MameyException
{
    public ContentGenerationNotFoundException(ContentGenerationId contentgenerationId)
        : base($"ContentGeneration with ID: '{contentgenerationId.Value}' was not found.")
        => ContentGenerationId = contentgenerationId;

    public ContentGenerationId ContentGenerationId { get; }
}

