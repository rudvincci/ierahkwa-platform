using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.Parents.Application.Events;

[Contract]
internal record ParentAdded(Guid ParentId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

