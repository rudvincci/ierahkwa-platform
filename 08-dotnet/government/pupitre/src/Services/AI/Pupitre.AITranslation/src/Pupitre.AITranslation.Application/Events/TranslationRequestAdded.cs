using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.AITranslation.Application.Events;

[Contract]
internal record TranslationRequestAdded(Guid TranslationRequestId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

