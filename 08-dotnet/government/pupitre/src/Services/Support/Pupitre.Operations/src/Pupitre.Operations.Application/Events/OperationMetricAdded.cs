using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.Operations.Application.Events;

[Contract]
internal record OperationMetricAdded(Guid OperationMetricId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

