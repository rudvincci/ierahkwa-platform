using Mamey.CQRS;
using Pupitre.AITranslation.Domain.Entities;

namespace Pupitre.AITranslation.Domain.Events;

internal record TranslationRequestModified(TranslationRequest TranslationRequest): IDomainEvent;

