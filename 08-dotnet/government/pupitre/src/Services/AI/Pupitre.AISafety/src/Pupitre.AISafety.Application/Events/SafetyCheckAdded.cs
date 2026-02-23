using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.AISafety.Application.Events;

[Contract]
internal record SafetyCheckAdded(Guid SafetyCheckId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

