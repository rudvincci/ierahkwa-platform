using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.AIBehavior.Application.Events;

[Contract]
internal record BehaviorAdded(Guid BehaviorId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

