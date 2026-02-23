using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AIContent.Domain.Entities;

namespace Pupitre.AIContent.Application.Exceptions;

internal class ContentGenerationAlreadyExistsException : MameyException
{
    public ContentGenerationAlreadyExistsException(ContentGenerationId contentgenerationId)
        : base($"ContentGeneration with ID: '{contentgenerationId.Value}' already exists.")
        => ContentGenerationId = contentgenerationId;

    public ContentGenerationId ContentGenerationId { get; }
}
