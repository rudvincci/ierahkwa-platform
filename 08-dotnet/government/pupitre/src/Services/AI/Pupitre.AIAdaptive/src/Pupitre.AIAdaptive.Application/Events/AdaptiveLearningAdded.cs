using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.AIAdaptive.Application.Events;

[Contract]
internal record AdaptiveLearningAdded(Guid AdaptiveLearningId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

