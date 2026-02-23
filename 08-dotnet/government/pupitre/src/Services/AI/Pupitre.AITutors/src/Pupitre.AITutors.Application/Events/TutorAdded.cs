using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.AITutors.Application.Events;

[Contract]
internal record TutorAdded(Guid TutorId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

