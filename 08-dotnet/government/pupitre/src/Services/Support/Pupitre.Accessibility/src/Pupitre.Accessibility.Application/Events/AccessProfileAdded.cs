using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.Accessibility.Application.Events;

[Contract]
internal record AccessProfileAdded(Guid AccessProfileId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

