using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.AIRecommendations.Application.Events;

[Contract]
internal record AIRecommendationAdded(Guid AIRecommendationId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

