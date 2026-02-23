using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.AIAssessments.Application.Events;

[Contract]
internal record AIAssessmentAdded(Guid AIAssessmentId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

