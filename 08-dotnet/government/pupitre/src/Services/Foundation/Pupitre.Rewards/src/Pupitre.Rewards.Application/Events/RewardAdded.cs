using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.Rewards.Application.Events;

[Contract]
internal record RewardAdded(Guid RewardId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

