using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.Progress.Application.Events;

[Contract]
internal record LearningProgressAdded(Guid LearningProgressId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

