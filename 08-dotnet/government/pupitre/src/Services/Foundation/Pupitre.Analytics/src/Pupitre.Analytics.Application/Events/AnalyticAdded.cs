using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.Analytics.Application.Events;

[Contract]
internal record AnalyticAdded(Guid AnalyticId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

