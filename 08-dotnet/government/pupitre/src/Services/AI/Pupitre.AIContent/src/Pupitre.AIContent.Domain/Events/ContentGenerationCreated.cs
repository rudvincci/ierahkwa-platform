using Mamey.CQRS;
using Pupitre.AIContent.Domain.Entities;

namespace Pupitre.AIContent.Domain.Events;

internal record ContentGenerationCreated(ContentGeneration ContentGeneration) : IDomainEvent;

