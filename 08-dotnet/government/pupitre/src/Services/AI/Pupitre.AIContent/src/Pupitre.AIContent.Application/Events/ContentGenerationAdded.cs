using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.AIContent.Application.Events;

[Contract]
internal record ContentGenerationAdded(Guid ContentGenerationId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

