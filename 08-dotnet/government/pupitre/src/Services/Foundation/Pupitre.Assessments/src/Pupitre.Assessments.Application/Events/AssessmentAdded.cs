using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.Assessments.Application.Events;

[Contract]
internal record AssessmentAdded(Guid AssessmentId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

