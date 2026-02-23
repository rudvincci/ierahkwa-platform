using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Mamey.Government.Identity.Application.Events;

[Contract]
internal record SubjectAdded(Guid SubjectId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

