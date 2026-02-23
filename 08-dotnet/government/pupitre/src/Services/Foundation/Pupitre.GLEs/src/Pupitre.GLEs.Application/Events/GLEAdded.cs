using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.GLEs.Application.Events;

[Contract]
internal record GLEAdded(Guid GLEId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

