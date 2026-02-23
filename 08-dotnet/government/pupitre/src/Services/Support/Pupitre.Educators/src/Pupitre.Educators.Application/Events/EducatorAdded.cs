using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.Educators.Application.Events;

[Contract]
internal record EducatorAdded(Guid EducatorId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

