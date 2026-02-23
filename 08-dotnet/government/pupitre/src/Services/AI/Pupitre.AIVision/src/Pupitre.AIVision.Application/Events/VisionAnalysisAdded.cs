using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.AIVision.Application.Events;

[Contract]
internal record VisionAnalysisAdded(Guid VisionAnalysisId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

