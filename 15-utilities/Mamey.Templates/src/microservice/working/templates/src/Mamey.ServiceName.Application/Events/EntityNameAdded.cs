using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Mamey.ServiceName.Application.Events;

[Contract]
internal record EntityNameAdded(Guid EntityNameId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

