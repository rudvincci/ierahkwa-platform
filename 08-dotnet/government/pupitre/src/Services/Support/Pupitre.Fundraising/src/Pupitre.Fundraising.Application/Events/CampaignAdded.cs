using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.Fundraising.Application.Events;

[Contract]
internal record CampaignAdded(Guid CampaignId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

