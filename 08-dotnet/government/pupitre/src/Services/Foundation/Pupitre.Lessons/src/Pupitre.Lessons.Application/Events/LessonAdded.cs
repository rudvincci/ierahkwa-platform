using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.Lessons.Application.Events;

[Contract]
internal record LessonAdded(Guid LessonId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

