using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.Ministries.Application.Events;

[Contract]
internal record MinistryDataAdded(Guid MinistryDataId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

