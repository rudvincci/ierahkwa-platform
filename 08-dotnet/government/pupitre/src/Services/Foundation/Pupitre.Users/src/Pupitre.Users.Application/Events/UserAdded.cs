using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.Users.Application.Events;

[Contract]
internal record UserAdded(Guid UserId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

