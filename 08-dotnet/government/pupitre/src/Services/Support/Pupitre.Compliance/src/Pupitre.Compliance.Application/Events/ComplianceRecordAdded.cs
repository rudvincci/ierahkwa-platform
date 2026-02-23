using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.Compliance.Application.Events;

[Contract]
internal record ComplianceRecordAdded(Guid ComplianceRecordId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

