using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.Aftercare.Application.Events;

[Contract]
internal record AftercarePlanAdded(Guid AftercarePlanId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

